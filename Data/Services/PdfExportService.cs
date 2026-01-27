using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SecureJournal.Data.Models;

namespace SecureJournal.Data.Services
{
    public class PdfExportService
    {
        private readonly JournalEntryService _entries;

        public PdfExportService(JournalEntryService entries)
        {
            _entries = entries;

            // QuestPDF requires setting a license type
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<string> ExportRangeAsync(DateTime from, DateTime to)
        {
            var list = await _entries.GetRangeAsync(from, to);

            var folder = Path.Combine(FileSystem.AppDataDirectory, "Exports");
            Directory.CreateDirectory(folder);

            var fileName = $"Journal_{from:yyyyMMdd}_{to:yyyyMMdd}_{DateTime.Now:HHmmss}.pdf";
            var path = Path.Combine(folder, fileName);

            ExportToPath(list, path);
            return path;
        }

        private static void ExportToPath(List<JournalEntry> entries, string filePath)
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(25);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Text("SecureJournal Export").FontSize(18).SemiBold();

                    page.Content().Column(col =>
                    {
                        col.Spacing(10);

                        if (entries.Count == 0)
                        {
                            col.Item().Text("No entries found for this date range.");
                            return;
                        }

                        foreach (var e in entries.OrderBy(x => x.EntryDate))
                        {
                            col.Item().Border(1).Padding(10).Column(card =>
                            {
                                card.Spacing(4);

                                card.Item().Text($"{e.EntryDate:yyyy-MM-dd}  •  {e.Title}").SemiBold();

                                card.Item().Text($"Category: {e.Category}");
                                card.Item().Text($"Mood: {e.PrimaryMood}"
                                    + (string.IsNullOrWhiteSpace(e.SecondaryMood1) ? "" : $", {e.SecondaryMood1}")
                                    + (string.IsNullOrWhiteSpace(e.SecondaryMood2) ? "" : $", {e.SecondaryMood2}")
                                );

                                if (!string.IsNullOrWhiteSpace(e.TagsCsv))
                                    card.Item().Text($"Tags: {e.TagsCsv}");

                                card.Item().LineHorizontal(1);

                                // Markdown stored; export plain text-ish (keep simple)
                                var body = (e.Markdown ?? "").Trim();
                                if (body.Length == 0) body = "(empty)";

                                card.Item().Text(body);
                            });
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Generated: ");
                        x.Span(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    });
                });
            }).GeneratePdf(filePath);
        }
    }
}
