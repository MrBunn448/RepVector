namespace RepVector.UI.ViewModels;

public class WorkoutDetailViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public List<ExerciseViewModel> Exercises { get; set; } = new();
}