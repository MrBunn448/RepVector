namespace WorkoutTracker.Models;

/// <summary>
/// Represents a physical exercise that can be performed in a workout.
/// </summary>
public class Exercise
{

    public int Id { get; set; }

    public int? UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Type { get; set; } = "Other"; 

    public int? PrimaryMuscleGroupId { get; set; }

    public bool IsPredefined { get; set; }

    public DateTime CreatedAt { get; set; }

    public MuscleGroup? PrimaryMuscleGroup { get; set; }
}
