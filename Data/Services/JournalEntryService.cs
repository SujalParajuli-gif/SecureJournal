using SecureJournal.Data.Models;
using SQLite;
namespace SecureJournal.Data.Services
{
    // “JournalService” equivalent (renamed + rewritten)
    public class JournalEntryService
    {
        private readonly AppSqliteDb _db;

        public event Action? DataChanged;

        public JournalEntryService(AppSqliteDb db)
        {
            _db = db;
        }

        private static string ToDateKey(DateTime d) => d.ToString("yyyy-MM-dd");

        private static int CountWords(string? text)
        {
            var t = (text ?? "").Trim();
            if (t.Length == 0) return 0;

            // Simple beginner word count
            return t.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        private static string JoinTags(List<string> tags)
        {
            // Case-insensitive distinct + trimmed
            return string.Join(",",
                tags.Select(t => (t ?? "").Trim())
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
            );
        }

        private static List<string> SplitTags(string? csv)
        {
            return (csv ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public async Task<JournalEntry?> GetByIdAsync(int id)
        {
            await _db.EnsureReadyAsync();
            return await _db.Connection.Table<JournalEntry>().Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<JournalEntry?> GetByDateAsync(DateTime date)
        {
            await _db.EnsureReadyAsync();
            var key = ToDateKey(date.Date);
            return await _db.Connection.Table<JournalEntry>().Where(x => x.DateKey == key).FirstOrDefaultAsync();
        }

        public async Task<bool> HasEntryForDateAsync(DateTime date)
        {
            return (await GetByDateAsync(date)) != null;
        }

        public async Task<int> UpsertForDateAsync(
            DateTime date,
            string category,
            string title,
            string markdown,
            string primaryMood,
            List<string> secondaryMoods,
            List<string> tags
        )
        {
            await _db.EnsureReadyAsync();

            var day = date.Date;
            var key = ToDateKey(day);

            // Normalize moods: max 2, no duplicates, secondary != primary
            var cleanedSecondary = (secondaryMoods ?? new())
                .Where(m => !string.IsNullOrWhiteSpace(m))
                .Select(m => m.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Where(m => !string.Equals(m, primaryMood, StringComparison.OrdinalIgnoreCase))
                .Take(2)
                .ToList();

            var existing = await GetByDateAsync(day);
            if (existing == null)
            {
                var now = DateTime.Now;

                var entry = new JournalEntry
                {
                    DateKey = key,
                    EntryDate = day,
                    Category = category ?? "",
                    Title = title ?? "",
                    Markdown = markdown ?? "",
                    PrimaryMood = primaryMood ?? "",
                    SecondaryMood1 = cleanedSecondary.ElementAtOrDefault(0) ?? "",
                    SecondaryMood2 = cleanedSecondary.ElementAtOrDefault(1) ?? "",
                    TagsCsv = JoinTags(tags ?? new()),
                    CreatedAt = now,
                    UpdatedAt = now,
                    WordCount = CountWords(markdown)
                };

                try
                {
                    var newId = await _db.Connection.InsertAsync(entry);
                    DataChanged?.Invoke();
                    return entry.Id;
                }
                catch (SQLiteException)
                {
                    // If unique constraint hit, fallback to update
                    var again = await GetByDateAsync(day);
                    if (again != null)
                    {
                        return await UpdateExistingAsync(again.Id, category, title, markdown, primaryMood, cleanedSecondary, tags);
                    }
                    throw;
                }
            }

            return await UpdateExistingAsync(existing.Id, category, title, markdown, primaryMood, cleanedSecondary, tags);
        }

        private async Task<int> UpdateExistingAsync(
            int id,
            string category,
            string title,
            string markdown,
            string primaryMood,
            List<string> cleanedSecondary,
            List<string> tags
        )
        {
            var row = await GetByIdAsync(id);
            if (row == null) return 0;

            row.Category = category ?? "";
            row.Title = title ?? "";
            row.Markdown = markdown ?? "";
            row.PrimaryMood = primaryMood ?? "";
            row.SecondaryMood1 = cleanedSecondary.ElementAtOrDefault(0) ?? "";
            row.SecondaryMood2 = cleanedSecondary.ElementAtOrDefault(1) ?? "";
            row.TagsCsv = JoinTags(tags ?? new());
            row.UpdatedAt = DateTime.Now;
            row.WordCount = CountWords(markdown);

            await _db.Connection.UpdateAsync(row);
            DataChanged?.Invoke();
            return row.Id;
        }

        public async Task DeleteAsync(int id)
        {
            await _db.EnsureReadyAsync();
            await _db.Connection.DeleteAsync<JournalEntry>(id);
            DataChanged?.Invoke();
        }

        public async Task<List<JournalEntry>> GetAllAsync()
        {
            await _db.EnsureReadyAsync();
            return await _db.Connection.Table<JournalEntry>()
                .OrderByDescending(x => x.EntryDate)
                .ToListAsync();
        }

        public async Task<List<JournalEntry>> GetRangeAsync(DateTime from, DateTime to)
        {
            await _db.EnsureReadyAsync();

            var start = from.Date;
            var end = to.Date;

            return await _db.Connection.Table<JournalEntry>()
                .Where(x => x.EntryDate >= start && x.EntryDate <= end)
                .OrderByDescending(x => x.EntryDate)
                .ToListAsync();
        }

        public async Task<List<JournalEntry>> GetPageAsync(int pageIndex, int pageSize)
        {
            await _db.EnsureReadyAsync();
            var skip = Math.Max(0, pageIndex) * Math.Max(1, pageSize);

            return await _db.Connection.Table<JournalEntry>()
                .OrderByDescending(x => x.EntryDate)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<(int thisWeek, int currentStreak, string commonMood)> GetDashboardStatsAsync()
        {
            var entries = await GetAllAsync();

            var today = DateTime.Now.Date;
            var weekStart = today.AddDays(-6);

            var thisWeek = entries.Count(e => e.EntryDate >= weekStart && e.EntryDate <= today);

            var currentStreak = CalculateCurrentStreak(entries);
            var commonMood = MostCommonMood(entries);

            return (thisWeek, currentStreak, commonMood);
        }

        public async Task<(int current, int longest, int missed)> GetStreakStatsAsync()
        {
            var entries = await GetAllAsync();
            var (current, longest, missed) = CalculateStreaks(entries);
            return (current, longest, missed);
        }

        public async Task<Dictionary<string, int>> GetMoodCountsAsync(DateTime? from = null, DateTime? to = null)
        {
            var data = (from.HasValue && to.HasValue)
                ? await GetRangeAsync(from.Value, to.Value)
                : await GetAllAsync();

            var dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var e in data)
            {
                if (!string.IsNullOrWhiteSpace(e.PrimaryMood))
                {
                    dict[e.PrimaryMood] = dict.TryGetValue(e.PrimaryMood, out var n) ? n + 1 : 1;
                }
            }

            return dict.OrderByDescending(k => k.Value).ToDictionary(k => k.Key, v => v.Value, StringComparer.OrdinalIgnoreCase);
        }

        public async Task<Dictionary<string, int>> GetTagCountsAsync(DateTime? from = null, DateTime? to = null)
        {
            var data = (from.HasValue && to.HasValue)
                ? await GetRangeAsync(from.Value, to.Value)
                : await GetAllAsync();

            var dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var e in data)
            {
                foreach (var tag in SplitTags(e.TagsCsv))
                {
                    dict[tag] = dict.TryGetValue(tag, out var n) ? n + 1 : 1;
                }
            }

            return dict.OrderByDescending(k => k.Value).ToDictionary(k => k.Key, v => v.Value, StringComparer.OrdinalIgnoreCase);
        }

        public async Task<HashSet<string>> GetDateKeysWithEntriesAsync(DateTime month)
        {
            await _db.EnsureReadyAsync();

            var start = new DateTime(month.Year, month.Month, 1);
            var end = start.AddMonths(1).AddDays(-1);

            var rows = await _db.Connection.Table<JournalEntry>()
                .Where(x => x.EntryDate >= start && x.EntryDate <= end)
                .ToListAsync();

            return rows.Select(r => r.DateKey).ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        public async Task<List<JournalEntry>> SearchAsync(
     string? text,
     DateTime? from,
     DateTime? to,
     string? mood,
     string? tag
 )
        {
            await _db.EnsureReadyAsync();

            // sqlite-net-pcl returns AsyncTableQuery<T>, not IQueryable<T>
            var q = _db.Connection.Table<JournalEntry>();

            if (from.HasValue)
                q = q.Where(x => x.EntryDate >= from.Value.Date);

            if (to.HasValue)
                q = q.Where(x => x.EntryDate <= to.Value.Date);

            if (!string.IsNullOrWhiteSpace(mood))
                q = q.Where(x => x.PrimaryMood == mood);

            // Execute DB query first
            var list = await q
                .OrderByDescending(x => x.EntryDate)
                .ToListAsync();

            // Tag filter in-memory (simple like reference)
            if (!string.IsNullOrWhiteSpace(tag))
            {
                list = list.Where(e =>
                    (e.TagsCsv ?? "")
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Any(t => string.Equals(t.Trim(), tag.Trim(), StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            // Text filter in-memory (simple like reference)
            if (!string.IsNullOrWhiteSpace(text))
            {
                var t = text.Trim();
                list = list.Where(e =>
                    (e.Title ?? "").Contains(t, StringComparison.OrdinalIgnoreCase) ||
                    (e.Markdown ?? "").Contains(t, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            return list;
        }


        private static int CalculateCurrentStreak(List<JournalEntry> entries)
        {
            var set = entries.Select(e => e.EntryDate.Date).ToHashSet();
            var today = DateTime.Now.Date;

            var streak = 0;
            var d = today;

            while (set.Contains(d))
            {
                streak++;
                d = d.AddDays(-1);
            }

            return streak;
        }

        private static string MostCommonMood(List<JournalEntry> entries)
        {
            var dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var e in entries)
            {
                var m = (e.PrimaryMood ?? "").Trim();
                if (m.Length == 0) continue;

                dict[m] = dict.TryGetValue(m, out var n) ? n + 1 : 1;
            }

            return dict.Count == 0 ? "-" : dict.OrderByDescending(x => x.Value).First().Key;
        }

        private static (int current, int longest, int missed) CalculateStreaks(List<JournalEntry> entries)
        {
            var days = entries.Select(e => e.EntryDate.Date).Distinct().OrderBy(d => d).ToList();
            if (days.Count == 0) return (0, 0, 0);

            var today = DateTime.Now.Date;

            // Missed days = gaps between earliest and today
            var earliest = days.First();
            var totalDays = (today - earliest).Days + 1;
            var missed = Math.Max(0, totalDays - days.Count);

            var longest = 1;
            var run = 1;

            for (int i = 1; i < days.Count; i++)
            {
                if (days[i] == days[i - 1].AddDays(1))
                {
                    run++;
                    if (run > longest) longest = run;
                }
                else
                {
                    run = 1;
                }
            }

            var set = days.ToHashSet();
            var current = 0;
            var d = today;
            while (set.Contains(d))
            {
                current++;
                d = d.AddDays(-1);
            }

            return (current, longest, missed);
        }
    }
}
