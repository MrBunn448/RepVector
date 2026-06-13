using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Abstractions.Repositories;

/// Defines the contract for data access operations related to user preferences.
public interface IUserPreferencesRepository
{
    /// Retrieves preferences for a specific user.
    /// <param name="userId">The ID of the user.</param>
    /// <returns>The preferences entity if found, otherwise null.</returns>
    Task<UserPreferences?> GetByUserIdAsync(int userId);

    /// Saves user preferences by creating a new record or updating an existing one.
    /// <param name="userPreferences">The preference data to persist.</param>
    Task CreateOrUpdateAsync(UserPreferences userPreferences);
}
