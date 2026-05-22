using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Abstractions.Repositories;

/// <summary>
/// Defines the contract for data access operations related to user preferences.
/// </summary>
public interface IUserPreferencesRepository
{
    /// <summary>
    /// Retrieves preferences for a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>The preferences entity if found, otherwise null.</returns>
    Task<UserPreferences?> GetByUserIdAsync(int userId);

    /// <summary>
    /// Saves user preferences by creating a new record or updating an existing one.
    /// </summary>
    /// <param name="userPreferences">The preference data to persist.</param>
    Task CreateOrUpdateAsync(UserPreferences userPreferences);
}
