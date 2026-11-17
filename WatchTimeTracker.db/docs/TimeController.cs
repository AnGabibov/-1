using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace WatchTimeTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimeController : ControllerBase
    {
        private readonly string _connectionString = "Data Source=TimeTracker.db";

        [HttpPost("start")]
        public IActionResult StartWork([FromBody] string projectName = "Проект А")
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var cmdProject = connection.CreateCommand();
            cmdProject.CommandText = "INSERT OR IGNORE INTO Projects (Name) VALUES ($name); SELECT Id FROM Projects WHERE Name = $name;";
            cmdProject.Parameters.AddWithValue("$name", projectName);
            int projectId = Convert.ToInt32(cmdProject.ExecuteScalar());

            var cmdLog = connection.CreateCommand();
            cmdLog.CommandText = "INSERT INTO TimeLogs (ProjectId, StartedAt) VALUES ($pid, $start); SELECT last_insert_rowid();";
            cmdLog.Parameters.AddWithValue("$pid", projectId);
            cmdLog.Parameters.AddWithValue("$start", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
            long logId = (long)cmdLog.ExecuteScalar();

            return Ok($"Таймер запущен! ID записи: {logId}");
        }

        [HttpPost("stop/{id}")]
        public IActionResult StopWork(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT StartedAt, EndedAt FROM TimeLogs WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", id);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return NotFound("Запись не найдена");

            var startedAtStr = reader.GetString(0);
            if (!reader.IsDBNull(1)) return BadRequest("Эта запись уже остановлена");

            DateTime startedAt = DateTime.Parse(startedAtStr);
            DateTime endedAt = DateTime.UtcNow;
            int duration = (int)(endedAt - startedAt).TotalSeconds;

            var cmdUpdate = connection.CreateCommand();
            cmdUpdate.CommandText = "UPDATE TimeLogs SET EndedAt = $end, DurationSeconds = $dur WHERE Id = $id";
            cmdUpdate.Parameters.AddWithValue("$end", endedAt.ToString("yyyy-MM-dd HH:mm:ss"));
            cmdUpdate.Parameters.AddWithValue("$dur", duration);
            cmdUpdate.Parameters.AddWithValue("$id", id);
            cmdUpdate.ExecuteNonQuery();

            return Ok($"Работа остановлена. Итоговое время: {endedAt - startedAt}");
        }

        [HttpGet("report")]
        public IActionResult GetReport()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT t.Id, p.Name as ProjectName, t.StartedAt, t.EndedAt, t.DurationSeconds
                FROM TimeLogs t
                JOIN Projects p ON p.Id = t.ProjectId
                ORDER BY t.StartedAt DESC";

            var result = new List<object>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new
                {
                    Id = reader.GetInt32(0),
                    ProjectName = reader.GetString(1),
                    StartTime = reader.GetString(2),
                    EndTime = reader.IsDBNull(3) ? null : reader.GetString(3),
                    DurationSeconds = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4)
                });
            }

            return Ok(result);
        }
    }
}
