namespace WorkoutTracker.Models;
public class WorkoutSetLog
{
    public int Id { get; set; }

    public int SessionId { get; set; }

    public int ExerciseId { get; set; }

    public int SetNumber { get; set; }

    public decimal Weight { get; set; }

    public int Reps { get; set; }

    public int? Rpe { get; set; } 

    public string SetType { get; set; } = "Normal"; 

    public DateTime CompletedAt { get; set; }

    public Exercise? Exercise { get; set; }
}
