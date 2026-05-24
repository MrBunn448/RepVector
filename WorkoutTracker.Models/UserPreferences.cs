namespace WorkoutTracker.Models;

public class UserPreferences
{
    public int UserId { get; set; }

    public string? Username { get; set; }

    public string WeightUnit { get; set; } = "KG"; 

    public string DistanceUnit { get; set; } = "KM";

    public DateTime UpdatedAt { get; set; }
}
