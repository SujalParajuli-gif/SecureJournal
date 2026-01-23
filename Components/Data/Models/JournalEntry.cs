using SQLite;

namespace SecureJournal.Data.Models;

public class JournalEntry
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // Using yyyy-MM-dd so it is easy to query and sort, and UNIQUE to enforce one entry per day
    [Indexed(Name = "IX_JournalEntry_EntryDateKey", Unique = true)]
    public string EntryDateKey { get; set; } = "";

    // Using this for entry category (Work, Health, etc.)
    public string Category { get; set; } = "";

    // Using this for search and list views
    public string Title { get; set; } = "";

    // Using this for markdown content
    public string Content { get; set; } = "";

    // Using this for system timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Using this for analytics word count trends
    public int WordCount { get; set; }
}
