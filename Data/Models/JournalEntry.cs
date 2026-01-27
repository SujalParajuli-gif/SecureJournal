using SQLite;

namespace SecureJournal.Data.Models
{
    public class JournalEntry
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // yyyy-MM-dd unique key = one entry per day
        [Indexed(Name = "IX_JournalEntry_DateKey", Unique = true)]
        public string DateKey { get; set; } = "";

        public DateTime EntryDate { get; set; }

        public string Category { get; set; } = "";
        public string Title { get; set; } = "";

        // Your UI writes Markdown, so store Markdown.
        public string Markdown { get; set; } = "";

        // Mood fields (same idea as reference: 1 primary + up to 2 secondary)
        public string PrimaryMood { get; set; } = "";
        public string SecondaryMood1 { get; set; } = "";
        public string SecondaryMood2 { get; set; } = "";

        // Tags stored as CSV (reference-style simplicity)
        public string TagsCsv { get; set; } = "";

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int WordCount { get; set; }
    }
}
