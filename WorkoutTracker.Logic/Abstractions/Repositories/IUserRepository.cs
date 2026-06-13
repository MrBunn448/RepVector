using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Abstractions.Repositories;

/// Defines the contract for data access operations related to users.
public interface IUserRepository
{
    /// Retrieves a user by their unique identifier.
    /// <param name="userId">The ID of the user.</param>
    /// <returns>The user entity if found, otherwise null.</returns>
    Task<User?> GetByIdAsync(int userId);

    /// Retrieves a user by their email address. Useful for authentication.
    /// <param name="email">The email of the user.</param>
    /// <returns>The user entity if found, otherwise null.</returns>
    Task<User?> GetByEmailAsync(string email);

    /// Persists a new user record in the database.
    /// <param name="user">The user data to save.</param>
    /// <returns>The ID of the newly created user.</returns>
    Task<int> CreateAsync(User user);

    /// Updates an existing user record.
    /// <param name="user">The user entity with updated values.</param>
    Task UpdateAsync(User user);

    /// Removes a user record from the database.
    /// <param name="userId">The ID of the user to delete.</param>
    Task DeleteAsync(int userId);
}
