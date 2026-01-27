using SQLite;

namespace SecureJournal.Data
{
    // SQLite database wrapper for opening and initializing the app database
    public class AppSqliteDb
    {
        // SQLite connection instance (created during initialization)
        private SQLiteAsyncConnection? _conn;

        // Initialization task (prevents running InitAsync multiple times)
        private Task? _initTask;

        // Exposes the SQLite connection after initialization
        public SQLiteAsyncConnection Connection
        {
            get
            {
                if (_conn == null)
                    throw new InvalidOperationException("DB not initialized yet.");

                return _conn;
            }
        }

        // Ensures the database is initialized before any queries run
        public Task EnsureReadyAsync()
        {
            _initTask ??= InitAsync();
            return _initTask;
        }

        // Creates the SQLite file and required tables if they do not exist
        private async Task InitAsync()
        {
            // Database file stored in the MAUI app data directory
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "SecureJournal.db3");

            // Async SQLite connection setup
            _conn = new SQLiteAsyncConnection(
                dbPath,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache
            );

            // Main table used by the journal app
            await _conn.CreateTableAsync<Models.JournalEntry>();
        }
    }
}
