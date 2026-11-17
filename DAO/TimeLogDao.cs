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
            cmd.CommandText = "SELECT Id, TaskId, StartedAt, EndedAt, DurationSeconds, Comment FROM TimeLogs WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", id);
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
            // ВАЖНО: в Microsoft.Data.Sqlite параметры привязываются по ИМЕНИ,
            // а не по позиции. Повторяющиеся "?" не работают — используем уникальные имена.
            cmd.CommandText = @"INSERT INTO TimeLogs (TaskId, StartedAt, EndedAt, DurationSeconds, Comment) 
                                VALUES ($taskId, $startedAt, $endedAt, $duration, $comment); SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("$taskId", t.TaskId);
            cmd.Parameters.AddWithValue("$startedAt", t.StartedAt.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("$endedAt", t.EndedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("$duration", t.DurationSeconds ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("$comment", t.Comment ?? (object)DBNull.Value);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void Update(TimeLog t)
        {
            using var conn = GetConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE TimeLogs SET TaskId = $taskId, StartedAt = $startedAt, EndedAt = $endedAt, 
                                DurationSeconds = $duration, Comment = $comment WHERE Id = $id";
            cmd.Parameters.AddWithValue("$taskId", t.TaskId);
            cmd.Parameters.AddWithValue("$startedAt", t.StartedAt.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("$endedAt", t.EndedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("$duration", t.DurationSeconds ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("$comment", t.Comment ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("$id", t.Id);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = GetConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM TimeLogs WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", id);
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
                WHERE tl.StartedAt >= $from AND tl.StartedAt < $to AND tl.EndedAt IS NOT NULL
                GROUP BY p.Id, p.Name
                ORDER BY 2 DESC";
            cmd.Parameters.AddWithValue("$from", from.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("$to", to.ToString("yyyy-MM-dd HH:mm:ss"));
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
