using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Abstractions.Repositories;

/// Defines the contract for data access operations related to workout-exercise associations.
public interface IWorkoutExerciseRepository
{
    /// Retrieves all exercises linked to a specific workout.
    /// <param name="workoutId">The ID of the workout template.</param>
    /// <returns>A list of workout-exercise association entities.</returns>
    Task<List<WorkoutExercise>> GetByWorkoutIdAsync(int workoutId);

    /// Retrieves a specific workout-exercise link by its ID.
    /// <param name="workoutExerciseId">The ID of the link.</param>
    /// <returns>The association entity if found, otherwise null.</returns>
    Task<WorkoutExercise?> GetByIdAsync(int workoutExerciseId);

    /// Persists a new association between a workout and an exercise.
    /// <param name="workoutExercise">The association data to save.</param>
    /// <returns>The ID of the newly created association.</returns>
    Task<int> CreateAsync(WorkoutExercise workoutExercise);

    /// Updates an existing workout-exercise association record.
    /// <param name="workoutExercise">The association entity with updated values.</param>
    Task UpdateAsync(WorkoutExercise workoutExercise);

    /// Removes a workout-exercise association record from the database.
    /// <param name="workoutExerciseId">The ID of the association to delete.</param>
    Task DeleteAsync(int workoutExerciseId);
}