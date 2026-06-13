using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Abstractions.Repositories;

/// Defines the contract for data access operations related to workout sessions and set logging.
public interface IWorkoutSessionRepository
{
    /// Retrieves a specific workout session by its ID.
    /// <param name="workoutSessionId">The ID of the session.</param>
    /// <returns>The session entity if found, otherwise null.</returns>
    Task<WorkoutSession?> GetByIdAsync(int workoutSessionId);

    /// Retrieves all historical sessions belonging to a specific user.
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A list of session entities.</returns>
    Task<List<WorkoutSession>> GetByUserIdAsync(int userId);

    /// Finds the currently active session for a specific user.
    /// <param name="userId">The ID of the user.</param>
    /// <returns>The active session entity if one exists, otherwise null.</returns>
    Task<WorkoutSession?> GetActiveSessionByUserIdAsync(int userId);

    /// Persists a new workout session record in the database.
    /// <param name="session">The session data to save.</param>
    /// <returns>The ID of the newly created session.</returns>
    Task<int> CreateAsync(WorkoutSession session);

    /// Updates an existing workout session record.
    /// <param name="session">The session entity with updated values.</param>
    Task UpdateAsync(WorkoutSession session);

    /// Removes a workout session and all its associated set logs from the database.
    /// <param name="workoutSessionId">The ID of the session to delete.</param>
    Task DeleteAsync(int workoutSessionId);

    /// Persists a performance set log for an exercise in a session.
    /// <param name="log">The set log data to save.</param>
    /// <returns>The ID of the newly created log.</returns>
    Task<int> AddSetLogAsync(WorkoutSetLog log);

    /// Retrieves all performance set logs recorded during a specific session.
    /// <param name="sessionId">The ID of the session.</param>
    /// <returns>A list of set log entities.</returns>
    Task<List<WorkoutSetLog>> GetSetLogsBySessionIdAsync(int sessionId);

    /// Deletes a specific set log by its ID.
    Task DeleteSetLogAsync(int logId);

    /// Deletes all set logs for a specific exercise in a session.
    Task DeleteExerciseLogsAsync(int sessionId, int exerciseId);
}

