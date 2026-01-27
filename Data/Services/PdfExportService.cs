using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SecureJournal.Data.Models;

namespace SecureJournal.Data.Services
{
    // PDF export service for saving entries as a PDF file (date range)
    public class PdfExportService
    {
        // Entry service used to load journal entries for exporting
        private readonly JournalEntryService _entries;

        public PdfExportService(JournalEntryService entries)
        {
            _entries = entries;

            // QuestPDF license setting (required by QuestPDF)
            QuestPDF.Settings.License = LicenseType.Community;
        }

        // Exports all entries between the given dates and returns the saved file path
        public async Task<string> ExportRangeAsync(DateTime from, DateTime to)
        {
            var list = await _entries.GetRangeAsync(from, to);

            // Export folder inside app data directory
            var folder = Path.Combine(FileSystem.AppDataDirectory, "Exports");
            Directory.CreateDirectory(folder);

            // File name includes from/to dates and time to prevent overwriting
            var fileName = $"Journal_{from:yyyyMMdd}_{to:yyyyMMdd}_{DateTime.Now:HHmmss}.pdf";
            var path = Path.Combine(folder, fileName);

            ExportToPath(list, path);
            return path;
        }

        // Builds the PDF document and writes it to disk
        private static void ExportToPath(List<JournalEntry> entries, string filePath)
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(25);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    // PDF title header
                    page.Header().Text("SecureJournal Export").FontSize(18).SemiBold();

                    // PDF body content
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

                                // Entry heading line
                                card.Item().Text($"{e.EntryDate:yyyy-MM-dd}  •  {e.Title}").SemiBold();

                                // Basic entry info
                                card.Item().Text($"Category: {e.Category}");
                                card.Item().Text($"Mood: {e.PrimaryMood}"
                                    + (string.IsNullOrWhiteSpace(e.SecondaryMood1) ? "" : $", {e.SecondaryMood1}")
                                    + (string.IsNullOrWhiteSpace(e.SecondaryMood2) ? "" : $", {e.SecondaryMood2}")
                                );

                                // Tags line
                                if (!string.IsNullOrWhiteSpace(e.TagsCsv))
                                    card.Item().Text($"Tags: {e.TagsCsv}");

                                card.Item().LineHorizontal(1);

                                // Entry body (stored as markdown, exported as plain text)
                                var body = (e.Markdown ?? "").Trim();
                                if (body.Length == 0) body = "(empty)";

                                card.Item().Text(body);
                            });
                        }
                    });

                    // Footer timestamp
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
