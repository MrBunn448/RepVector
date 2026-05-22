using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Abstractions.Repositories;

/// <summary>
/// Defines the contract for data access operations related to exercises.
/// </summary>
public interface IExerciseRepository
{
    /// <summary>
    /// Retrieves all exercises from the database that are either predefined or owned by the specified user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A list of exercise entities.</returns>
    Task<List<Exercise>> GetAllAsync(int userId);

    /// <summary>
    /// Retrieves a specific exercise by its unique identifier.
    /// </summary>
    /// <param name="exerciseId">The ID of the exercise.</param>
    /// <returns>The exercise entity if found, otherwise null.</returns>
    Task<Exercise?> GetByIdAsync(int exerciseId);

    /// <summary>
    /// Persists a new exercise in the database.
    /// </summary>
    /// <param name="exercise">The exercise data to save.</param>
    /// <returns>The ID of the newly created exercise.</returns>
    Task<int> CreateAsync(Exercise exercise);

    /// <summary>
    /// Updates an existing exercise record.
    /// </summary>
    /// <param name="exercise">The exercise entity with updated values.</param>
    Task UpdateAsync(Exercise exercise);

    /// <summary>
    /// Removes an exercise record from the database.
    /// </summary>
    /// <param name="exerciseId">The ID of the exercise to delete.</param>
    Task DeleteAsync(int exerciseId);
}
