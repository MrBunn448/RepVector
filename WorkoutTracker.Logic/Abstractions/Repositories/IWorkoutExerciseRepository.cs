using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Abstractions.Repositories;

/// <summary>
/// Defines the contract for data access operations related to workout-exercise associations.
/// </summary>
public interface IWorkoutExerciseRepository
{
    /// <summary>
    /// Retrieves all exercises linked to a specific workout.
    /// </summary>
    /// <param name="workoutId">The ID of the workout template.</param>
    /// <returns>A list of workout-exercise association entities.</returns>
    Task<List<WorkoutExercise>> GetByWorkoutIdAsync(int workoutId);

    /// <summary>
    /// Retrieves a specific workout-exercise link by its ID.
    /// </summary>
    /// <param name="workoutExerciseId">The ID of the link.</param>
    /// <returns>The association entity if found, otherwise null.</returns>
    Task<WorkoutExercise?> GetByIdAsync(int workoutExerciseId);

    /// <summary>
    /// Persists a new association between a workout and an exercise.
    /// </summary>
    /// <param name="workoutExercise">The association data to save.</param>
    /// <returns>The ID of the newly created association.</returns>
    Task<int> CreateAsync(WorkoutExercise workoutExercise);

    /// <summary>
    /// Updates an existing workout-exercise association record.
    /// </summary>
    /// <param name="workoutExercise">The association entity with updated values.</param>
    Task UpdateAsync(WorkoutExercise workoutExercise);

    /// <summary>
    /// Removes a workout-exercise association record from the database.
    /// </summary>
    /// <param name="workoutExerciseId">The ID of the association to delete.</param>
    Task DeleteAsync(int workoutExerciseId);
}