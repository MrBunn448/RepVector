namespace WorkoutTracker.Models;

public class WorkoutSession
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int? WorkoutId { get; set; }

    public string? WorkoutName { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime? FinishedAt { get; set; }

    public int TotalSeconds { get; set; }

    public string Status { get; set; } = "active";

    public List<WorkoutSetLog> SetLogs { get; set; } = new();
}
