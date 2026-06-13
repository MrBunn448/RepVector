using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Services;

public interface IPreferenceService
{

    Task<Result<UserPreferences>> GetPreferencesAsync(int userId);

    Task<Result> SavePreferencesAsync(UserPreferences userPreferences, User editor);

    string FormatWeight(decimal weight, string measurementUnit);

    string FormatDistance(decimal distance, string measurementUnit);
}
