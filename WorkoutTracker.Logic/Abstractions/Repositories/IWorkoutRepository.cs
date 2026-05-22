using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Abstractions.Repositories;

/// <summary>
/// Defines the contract for data access operations related to workout templates.
/// </summary>
public interface IWorkoutRepository
{
    /// <summary>
    /// Retrieves all workout templates that are either predefined or owned by the specified user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A list of workout templates.</returns>
    Task<List<Workout>> GetAllByUserIdAsync(int userId);

    /// <summary>
    /// Retrieves a specific workout template by its ID.
    /// </summary>
    /// <param name="workoutId">The ID of the workout template.</param>
    /// <returns>The workout entity if found, otherwise null.</returns>
    Task<Workout?> GetByIdAsync(int workoutId);

    /// <summary>
    /// Persists a new workout template record in the database.
    /// </summary>
    /// <param name="workout">The workout template data to save.</param>
    /// <returns>The ID of the newly created workout template.</returns>
    Task<int> CreateAsync(Workout workout);

    /// <summary>
    /// Updates an existing workout template record.
    /// </summary>
    /// <param name="workout">The workout entity with updated values.</param>
    Task UpdateAsync(Workout workout);

    /// <summary>
    /// Removes a workout template and its associated exercise links from the database.
    /// </summary>
    /// <param name="workoutId">The ID of the workout template to delete.</param>
    Task DeleteAsync(int workoutId);

    /// <summary>
    /// Retrieves a workout template along with its full list of exercises in a single operation.
    /// </summary>
    /// <param name="workoutId">The ID of the workout template.</param>
    /// <returns>The workout entity with nested Exercises populated if found, otherwise null.</returns>
    Task<Workout?> GetWorkoutWithExercisesAsync(int workoutId);
}
