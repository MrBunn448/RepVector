using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Services;

/// <summary>
/// Defines the contract for managing active and historical workout sessions.
/// </summary>
public interface IWorkoutSessionService
{

    Task<WorkoutSession?> GetByIdAsync(int workoutSessionId);

    Task<List<WorkoutSession>> GetUserSessionsAsync(int userId);

    Task<WorkoutSession?> GetActiveSessionAsync(int userId);
    
    Task<Result<int>> StartSessionAsync(int userId, int workoutId, bool cancelExisting = false);

    Task<Result> UpdateSessionStatusAsync(int sessionId, string status, User editor);

    Task<Result> SaveSetLogAsync(WorkoutSetLog log, User user);

    Task<List<WorkoutSetLog>> GetSessionLogsAsync(int sessionId);

    Task<Result> DeleteSessionAsync(int workoutSessionId, User deleter);
}
