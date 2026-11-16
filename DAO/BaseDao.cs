using Microsoft.Data.Sqlite;

namespace WatchTimeTracker.API.DAO
{
    public abstract class BaseDao
    {
        protected readonly string ConnectionString;

        protected BaseDao(string connectionString)
        {
            ConnectionString = connectionString;
        }

        protected SqliteConnection GetConnection()
        {
            var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            return conn;
        }
    }
}
