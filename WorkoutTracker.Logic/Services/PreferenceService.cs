using System.Globalization;
using WorkoutTracker.Logic.Abstractions.Repositories;
using WorkoutTracker.Logic.Abstractions.Services;
using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Services;

public class PreferenceService(
    IUserPreferencesRepository userPreferencesRepository,
    IAuthorizationService auth) : IPreferenceService
{
    /// Retrieves the preferences for a specific user.
    public Task<UserPreferences?> GetPreferencesAsync(int userId)
        => userPreferencesRepository.GetByUserIdAsync(userId);

    /// Saves or updates the preferences for a user. Validates authorization before saving.
    public async Task<Result> SavePreferencesAsync(UserPreferences userPreferences, User editor)
    {
        // Use Centralized Authorization
        var authResult = auth.CanModifyPreference(editor, userPreferences.UserId);
        if (authResult.IsFailure) return authResult;

        await userPreferencesRepository.CreateOrUpdateAsync(userPreferences);
        return Result.Success();
    }

    /// Formats a weight value based on the specified unit.
    public string FormatWeight(decimal weight, string measurementUnit)
    {
        if (measurementUnit == "LBS")
        {
            return $"{(weight * 2.20462m).ToString("F1", CultureInfo.InvariantCulture)} Lbs";
        }
        return $"{weight.ToString("F1", CultureInfo.InvariantCulture)} Kg";
    }

    /// Formats a distance value based on the specified unit.
    public string FormatDistance(decimal distance, string measurementUnit)
    {
        if (measurementUnit == "Miles")
        {
            return $"{(distance * 0.621371m).ToString("F1", CultureInfo.InvariantCulture)} Miles";
        }
        return $"{distance.ToString("F1", CultureInfo.InvariantCulture)} Km";
    }
}
