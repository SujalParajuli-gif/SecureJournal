using SQLite;

namespace SecureJournal.Data
{
    // Simple DB wrapper like the reference AppDatabase (but renamed + rewritten)
    public class AppSqliteDb
    {
        private SQLiteAsyncConnection? _conn;
        private Task? _initTask;

        public SQLiteAsyncConnection Connection
        {
            get
            {
                if (_conn == null) throw new InvalidOperationException("DB not initialized yet.");
                return _conn;
            }
        }

        public Task EnsureReadyAsync()
        {
            _initTask ??= InitAsync();
            return _initTask;
        }

        private async Task InitAsync()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "SecureJournal.db3");

            _conn = new SQLiteAsyncConnection(
                dbPath,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache
            );

            await _conn.CreateTableAsync<Models.JournalEntry>();
        }
    }
}
