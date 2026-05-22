namespace WorkoutTracker.Models;
public class WorkoutExercise
{
    public int Id { get; set; }

    public int WorkoutId { get; set; }

    public int ExerciseId { get; set; }

    public int TargetSets { get; set; }

    public int TargetReps { get; set; }

    public int? TargetRpe { get; set; }

    public int SortOrder { get; set; }

    public Exercise? Exercise { get; set; }
}
