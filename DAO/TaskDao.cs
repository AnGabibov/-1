using Microsoft.Data.Sqlite;
using WatchTimeTracker.API.Models;

namespace WatchTimeTracker.API.DAO
{
    public class TaskDao : BaseDao
    {
        public TaskDao(string cs) : base(cs) { }

        public List<TaskItem> GetAll()
        {
            var list = new List<TaskItem>();
            using var conn = GetConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, ProjectId, Name, Description, Status FROM Tasks";
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new TaskItem
                {
                    Id = r.GetInt32(0),
                    ProjectId = r.GetInt32(1),
                    Name = r.GetString(2),
                    Description = r.IsDBNull(3) ? null : r.GetString(3),
                    Status = r.GetString(4)
                });
            }
            return list;
        }

        public TaskItem? GetById(int id)
        {
            using var conn = GetConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, ProjectId, Name, Description, Status FROM Tasks WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", id);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return new TaskItem
            {
                Id = r.GetInt32(0),
                ProjectId = r.GetInt32(1),
                Name = r.GetString(2),
                Description = r.IsDBNull(3) ? null : r.GetString(3),
                Status = r.GetString(4)
            };
        }

        public int Insert(TaskItem t)
        {
            using var conn = GetConnection();
            var cmd = conn.CreateCommand();
            // ВАЖНО: в Microsoft.Data.Sqlite параметры привязываются по ИМЕНИ,
            // а не по позиции. Повторяющиеся "?" не работают — используем уникальные имена.
            cmd.CommandText = @"INSERT INTO Tasks (ProjectId, Name, Description, Status) 
                                VALUES ($projectId, $name, $description, $status); SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("$projectId", t.ProjectId);
            cmd.Parameters.AddWithValue("$name", t.Name);
            cmd.Parameters.AddWithValue("$description", t.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("$status", t.Status);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void Update(TaskItem t)
        {
            using var conn = GetConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Tasks SET ProjectId = $projectId, Name = $name, Description = $description, Status = $status WHERE Id = $id";
            cmd.Parameters.AddWithValue("$projectId", t.ProjectId);
            cmd.Parameters.AddWithValue("$name", t.Name);
            cmd.Parameters.AddWithValue("$description", t.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("$status", t.Status);
            cmd.Parameters.AddWithValue("$id", t.Id);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = GetConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Tasks WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
        }
    }
}
