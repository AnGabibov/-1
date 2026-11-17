using Microsoft.Data.Sqlite;
using WatchTimeTracker.API.Models;

namespace WatchTimeTracker.API.DAO
{
    public class ProjectDao : BaseDao
    {
        public ProjectDao(string cs) : base(cs) { }

        public List<Project> GetAll()
        {
            var list = new List<Project>();
            using var conn = GetConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, ClientId, Name, Description, Color, HourlyRate, Status FROM Projects";
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new Project
                {
                    Id = r.GetInt32(0),
                    ClientId = r.IsDBNull(1) ? null : r.GetInt32(1),
                    Name = r.GetString(2),
                    Description = r.IsDBNull(3) ? null : r.GetString(3),
                    Color = r.GetString(4),
                    HourlyRate = r.IsDBNull(5) ? null : r.GetDouble(5),
                    Status = r.GetString(6)
                });
            }
            return list;
        }

        public Project? GetById(int id)
        {
            using var conn = GetConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, ClientId, Name, Description, Color, HourlyRate, Status FROM Projects WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", id);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return new Project
            {
                Id = r.GetInt32(0),
                ClientId = r.IsDBNull(1) ? null : r.GetInt32(1),
                Name = r.GetString(2),
                Description = r.IsDBNull(3) ? null : r.GetString(3),
                Color = r.GetString(4),
                HourlyRate = r.IsDBNull(5) ? null : r.GetDouble(5),
                Status = r.GetString(6)
            };
        }

        public int Insert(Project p)
        {
            using var conn = GetConnection();
            var cmd = conn.CreateCommand();
            // ВАЖНО: в Microsoft.Data.Sqlite параметры привязываются по ИМЕНИ,
            // а не по позиции. Повторяющиеся "?" не работают — используем уникальные имена.
            cmd.CommandText = @"INSERT INTO Projects (ClientId, Name, Description, Color, HourlyRate, Status) 
                                VALUES ($clientId, $name, $description, $color, $hourlyRate, $status); SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("$clientId", p.ClientId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("$name", p.Name);
            cmd.Parameters.AddWithValue("$description", p.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("$color", p.Color);
            cmd.Parameters.AddWithValue("$hourlyRate", p.HourlyRate ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("$status", p.Status);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void Update(Project p)
        {
            using var conn = GetConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE Projects SET ClientId = $clientId, Name = $name, Description = $description, 
                                Color = $color, HourlyRate = $hourlyRate, Status = $status WHERE Id = $id";
            cmd.Parameters.AddWithValue("$clientId", p.ClientId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("$name", p.Name);
            cmd.Parameters.AddWithValue("$description", p.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("$color", p.Color);
            cmd.Parameters.AddWithValue("$hourlyRate", p.HourlyRate ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("$status", p.Status);
            cmd.Parameters.AddWithValue("$id", p.Id);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = GetConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Projects WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
        }
    }
}
