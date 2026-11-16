namespace WatchTimeTracker.API.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ContactInfo { get; set; }
    }

    public class Project
    {
        public int Id { get; set; }
        public int? ClientId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Color { get; set; } = "#3B82F6";
        public double? HourlyRate { get; set; }
        public string Status { get; set; } = "active";
    }

    public class TaskItem
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = "open";
    }

    public class TimeLog
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public int? DurationSeconds { get; set; }
        public string? Comment { get; set; }
    }
}
