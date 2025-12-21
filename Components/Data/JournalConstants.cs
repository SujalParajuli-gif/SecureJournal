namespace SecureJournal.Components.Data;

public static class JournalConstants
{
    public static readonly string[] EntryCategories =
    {
        "Work", "Health", "Travel", "Fitness", "Studies", "Family", "Personal Growth", "Reflection", "Other"
    };

    public static readonly Dictionary<string, string[]> MoodGroups = new()
    {
        ["Positive"] = new[] { "Happy", "Excited", "Relaxed", "Grateful", "Confident" },
        ["Neutral"] = new[] { "Calm", "Thoughtful", "Curious", "Nostalgic", "Bored" },
        ["Negative"] = new[] { "Sad", "Angry", "Stressed", "Lonely", "Anxious" }
    };

    public static readonly string[] PrebuiltTags =
    {
        "Work","Career","Studies","Family","Friends","Relationships","Health","Fitness","Personal Growth","Self-care",
        "Hobbies","Travel","Nature","Finance","Spirituality","Birthday","Holiday","Vacation","Celebration","Exercise",
        "Reading","Writing","Cooking","Meditation","Yoga","Music","Shopping","Parenting","Projects","Planning","Reflection"
    };
}
