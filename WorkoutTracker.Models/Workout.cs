namespace WorkoutTracker.Models;

public class Workout
{

    public int Id { get; set; }

    public int? UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsPredefined { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<WorkoutExercise> Exercises { get; set; } = new();
}
