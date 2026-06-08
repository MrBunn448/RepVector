using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Abstractions.Repositories;

/// <summary>
/// Defines the contract for data access operations related to workout sessions and set logging.
/// </summary>
public interface IWorkoutSessionRepository
{
    /// <summary>
    /// Retrieves a specific workout session by its ID.
    /// </summary>
    /// <param name="workoutSessionId">The ID of the session.</param>
    /// <returns>The session entity if found, otherwise null.</returns>
    Task<WorkoutSession?> GetByIdAsync(int workoutSessionId);

    /// <summary>
    /// Retrieves all historical sessions belonging to a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A list of session entities.</returns>
    Task<List<WorkoutSession>> GetByUserIdAsync(int userId);

    /// <summary>
    /// Finds the currently active session for a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>The active session entity if one exists, otherwise null.</returns>
    Task<WorkoutSession?> GetActiveSessionByUserIdAsync(int userId);

    /// <summary>
    /// Persists a new workout session record in the database.
    /// </summary>
    /// <param name="session">The session data to save.</param>
    /// <returns>The ID of the newly created session.</returns>
    Task<int> CreateAsync(WorkoutSession session);

    /// <summary>
    /// Updates an existing workout session record.
    /// </summary>
    /// <param name="session">The session entity with updated values.</param>
    Task UpdateAsync(WorkoutSession session);

    /// <summary>
    /// Removes a workout session and all its associated set logs from the database.
    /// </summary>
    /// <param name="workoutSessionId">The ID of the session to delete.</param>
    Task DeleteAsync(int workoutSessionId);

    /// <summary>
    /// Persists a performance set log for an exercise in a session.
    /// </summary>
    /// <param name="log">The set log data to save.</param>
    /// <returns>The ID of the newly created log.</returns>
    Task<int> AddSetLogAsync(WorkoutSetLog log);

    /// <summary>
    /// Retrieves all performance set logs recorded during a specific session.
    /// </summary>
    /// <param name="sessionId">The ID of the session.</param>
    /// <returns>A list of set log entities.</returns>
    Task<List<WorkoutSetLog>> GetSetLogsBySessionIdAsync(int sessionId);

    /// <summary>
    /// Deletes a specific set log by its ID.
    /// </summary>
    Task DeleteSetLogAsync(int logId);

    /// <summary>
    /// Deletes all set logs for a specific exercise in a session.
    /// </summary>
    Task DeleteExerciseLogsAsync(int sessionId, int exerciseId);
}

