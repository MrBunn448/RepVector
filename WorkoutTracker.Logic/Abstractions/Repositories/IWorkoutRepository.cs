using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Abstractions.Repositories;

/// Defines the contract for data access operations related to workout templates.
public interface IWorkoutRepository
{
    /// Retrieves all workout templates that are either predefined or owned by the specified user.
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A list of workout templates.</returns>
    Task<List<Workout>> GetAllByUserIdAsync(int userId);

    /// Retrieves a specific workout template by its ID.
    /// <param name="workoutId">The ID of the workout template.</param>
    /// <returns>The workout entity if found, otherwise null.</returns>
    Task<Workout?> GetByIdAsync(int workoutId);

    /// Persists a new workout template record in the database.
    /// <param name="workout">The workout template data to save.</param>
    /// <returns>The ID of the newly created workout template.</returns>
    Task<int> CreateAsync(Workout workout);

    /// Updates an existing workout template record.
    /// <param name="workout">The workout entity with updated values.</param>
    Task UpdateAsync(Workout workout);

    /// Removes a workout template and its associated exercise links from the database.
    /// <param name="workoutId">The ID of the workout template to delete.</param>
    Task DeleteAsync(int workoutId);

    /// Retrieves a workout template along with its full list of exercises in a single operation.
    /// <param name="workoutId">The ID of the workout template.</param>
    /// <returns>The workout entity with nested Exercises populated if found, otherwise null.</returns>
    Task<Workout?> GetWorkoutWithExercisesAsync(int workoutId);
}
