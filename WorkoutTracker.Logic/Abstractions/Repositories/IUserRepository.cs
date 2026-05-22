using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Abstractions.Repositories;

/// <summary>
/// Defines the contract for data access operations related to users.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>The user entity if found, otherwise null.</returns>
    Task<User?> GetByIdAsync(int userId);

    /// <summary>
    /// Retrieves a user by their email address. Useful for authentication.
    /// </summary>
    /// <param name="email">The email of the user.</param>
    /// <returns>The user entity if found, otherwise null.</returns>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// Persists a new user record in the database.
    /// </summary>
    /// <param name="user">The user data to save.</param>
    /// <returns>The ID of the newly created user.</returns>
    Task<int> CreateAsync(User user);

    /// <summary>
    /// Updates an existing user record.
    /// </summary>
    /// <param name="user">The user entity with updated values.</param>
    Task UpdateAsync(User user);

    /// <summary>
    /// Removes a user record from the database.
    /// </summary>
    /// <param name="userId">The ID of the user to delete.</param>
    Task DeleteAsync(int userId);
}
