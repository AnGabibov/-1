using Microsoft.Data.Sqlite;
using WatchTimeTracker.API.Models;

namespace WatchTimeTracker.API.DAO
{
    public class TimeLogDao : BaseDao
    {
        public TimeLogDao(string cs) : base(cs) { }

        public List<TimeLog> GetAll()
        {
            var list = new List<TimeLog>();
            using var conn = GetConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, TaskId, StartedAt, EndedAt, DurationSeconds, Comment FROM TimeLogs";
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new TimeLog
                {
                    Id = r.GetInt32(0),
                    TaskId = r.GetInt32(1),
                    StartedAt = DateTime.Parse(r.GetString(2)),
                    EndedAt = r.IsDBNull(3) ? null : DateTime.Parse(r.GetString(3)),
                    DurationSeconds = r.IsDBNull(4) ? null : r.GetInt32(4),
                    Comment = r.IsDBNull(5) ? null : r.GetString(5)
                });
            }
            return list;
        }

        public TimeLog? GetById(int id)
        {
            using var conn = GetConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, TaskId, StartedAt, EndedAt, DurationSeconds, Comment FROM TimeLogs WHERE Id = ?";
            cmd.Parameters.AddWithValue("?", id);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return new TimeLog
            {
                Id = r.GetInt32(0),
                TaskId = r.GetInt32(1),
                StartedAt = DateTime.Parse(r.GetString(2)),
                EndedAt = r.IsDBNull(3) ? null : DateTime.Parse(r.GetString(3)),
                DurationSeconds = r.IsDBNull(4) ? null : r.GetInt32(4),
                Comment = r.IsDBNull(5) ? null : r.GetString(5)
            };
        }

        public int Insert(TimeLog t)
        {
            using var conn = GetConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO TimeLogs (TaskId, StartedAt, EndedAt, DurationSeconds, Comment) 
                                VALUES (?, ?, ?, ?, ?); SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("?", t.TaskId);
            cmd.Parameters.AddWithValue("?", t.StartedAt.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("?", t.EndedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("?", t.DurationSeconds ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("?", t.Comment ?? (object)DBNull.Value);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void Update(TimeLog t)
        {
            using var conn = GetConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE TimeLogs SET TaskId = ?, StartedAt = ?, EndedAt = ?, 
                                DurationSeconds = ?, Comment = ? WHERE Id = ?";
            cmd.Parameters.AddWithValue("?", t.TaskId);
            cmd.Parameters.AddWithValue("?", t.StartedAt.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("?", t.EndedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("?", t.DurationSeconds ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("?", t.Comment ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("?", t.Id);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = GetConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM TimeLogs WHERE Id = ?";
            cmd.Parameters.AddWithValue("?", id);
            cmd.ExecuteNonQuery();
        }

        // ===== БИЗНЕС-ЗАПРОСЫ =====

        public List<(string ProjectName, double TotalHours)> GetHoursByProject(DateTime from, DateTime to)
        {
            var list = new List<(string, double)>();
            using var conn = GetConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT p.Name, SUM(tl.DurationSeconds) / 3600.0
                FROM TimeLogs tl
                JOIN Tasks t ON t.Id = tl.TaskId
                JOIN Projects p ON p.Id = t.ProjectId
                WHERE tl.StartedAt >= ? AND tl.StartedAt < ? AND tl.EndedAt IS NOT NULL
                GROUP BY p.Id, p.Name
                ORDER BY 2 DESC";
            cmd.Parameters.AddWithValue("?", from.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("?", to.ToString("yyyy-MM-dd HH:mm:ss"));
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add((r.GetString(0), r.GetDouble(1)));
            return list;
        }

        public List<(string TaskName, string ProjectName, string StartedAt)> GetActiveTimers()
        {
            var list = new List<(string, string, string)>();
            using var conn = GetConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT t.Name, p.Name, tl.StartedAt
                FROM TimeLogs tl
                JOIN Tasks t ON t.Id = tl.TaskId
                JOIN Projects p ON p.Id = t.ProjectId
                WHERE tl.EndedAt IS NULL";
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add((r.GetString(0), r.GetString(1), r.GetString(2)));
            return list;
        }
    }
}
