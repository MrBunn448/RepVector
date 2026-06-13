using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Abstractions.Repositories;

/// Defines the contract for data access operations related to exercises.
public interface IExerciseRepository
{
    /// Retrieves all exercises from the database that are either predefined or owned by the specified user.
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A list of exercise entities.</returns>
    Task<List<Exercise>> GetAllAsync(int userId);

    /// Retrieves a specific exercise by its unique identifier.
    /// <param name="exerciseId">The ID of the exercise.</param>
    /// <returns>The exercise entity if found, otherwise null.</returns>
    Task<Exercise?> GetByIdAsync(int exerciseId);

    /// Persists a new exercise in the database.
    /// <param name="exercise">The exercise data to save.</param>
    /// <returns>The ID of the newly created exercise.</returns>
    Task<int> CreateAsync(Exercise exercise);

    /// Updates an existing exercise record.
    /// <param name="exercise">The exercise entity with updated values.</param>
    Task UpdateAsync(Exercise exercise);

    /// Removes an exercise record from the database.
    /// <param name="exerciseId">The ID of the exercise to delete.</param>
    Task DeleteAsync(int exerciseId);
}
