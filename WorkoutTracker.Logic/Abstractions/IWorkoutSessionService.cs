using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Services;

/// Defines the contract for managing active and historical workout sessions.
public interface IWorkoutSessionService
{

    Task<Result<WorkoutSession>> GetByIdAsync(int workoutSessionId);

    Task<Result<List<WorkoutSession>>> GetUserSessionsAsync(int userId);

    Task<Result<WorkoutSession>> GetActiveSessionAsync(int userId);
    
    Task<Result<int>> StartSessionAsync(int userId, int workoutId, bool cancelExisting = false);

    Task<Result> UpdateSessionStatusAsync(int sessionId, string status, User editor);

    Task<Result<WorkoutSetLog>> SaveSetLogAsync(WorkoutSetLog log, User user);

    Task<Result> DeleteSetLogAsync(int sessionId, int logId, User user);

    Task<Result> DeleteExerciseLogsAsync(int sessionId, int exerciseId, User user);

    Task<Result<List<WorkoutSetLog>>> GetSessionLogsAsync(int sessionId);

    Task<Result> DeleteSessionAsync(int workoutSessionId, User deleter);
}
