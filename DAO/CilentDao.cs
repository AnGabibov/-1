using Microsoft.Data.Sqlite;
using WatchTimeTracker.API.Models;

namespace WatchTimeTracker.API.DAO
{
    public class ClientDao : BaseDao
    {
        public ClientDao(string cs) : base(cs) { }

        public List<Client> GetAll()
        {
            var list = new List<Client>();
            using var conn = GetConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Name, ContactInfo FROM Clients";
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new Client
                {
                    Id = r.GetInt32(0),
                    Name = r.GetString(1),
                    ContactInfo = r.IsDBNull(2) ? null : r.GetString(2)
                });
            }
            return list;
        }

        public Client? GetById(int id)
        {
            using var conn = GetConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Name, ContactInfo FROM Clients WHERE Id = ?";
            cmd.Parameters.AddWithValue("?", id);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return new Client { Id = r.GetInt32(0), Name = r.GetString(1), ContactInfo = r.IsDBNull(2) ? null : r.GetString(2) };
        }

        public int Insert(Client c)
        {
            using var conn = GetConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Clients (Name, ContactInfo) VALUES (?, ?); SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("?", c.Name);
            cmd.Parameters.AddWithValue("?", c.ContactInfo ?? (object)DBNull.Value);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void Update(Client c)
        {
            using var conn = GetConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Clients SET Name = ?, ContactInfo = ? WHERE Id = ?";
            cmd.Parameters.AddWithValue("?", c.Name);
            cmd.Parameters.AddWithValue("?", c.ContactInfo ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("?", c.Id);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = GetConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Clients WHERE Id = ?";
            cmd.Parameters.AddWithValue("?", id);
            cmd.ExecuteNonQuery();
        }
    }
}
