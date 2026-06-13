using WorkoutTracker.Logic.Abstractions.Repositories;
using WorkoutTracker.Logic.Abstractions.Services;
using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Services;
public class WorkoutSessionService(
    IWorkoutSessionRepository workoutSessionRepository,
    IWorkoutRepository workoutRepository,
    IAuthorizationService auth) : IWorkoutSessionService
{
    /// Retrieves a specific workout session by its ID.
    public async Task<Result<WorkoutSession>> GetByIdAsync(int workoutSessionId)
    {
        try
        {
            var session = await workoutSessionRepository.GetByIdAsync(workoutSessionId);
            if (session == null) return Result<WorkoutSession>.NotFound();
            return Result<WorkoutSession>.Success(session);
        }
        catch (Exception ex)
        {
            return Result<WorkoutSession>.Failure($"Failed to retrieve session: {ex.Message}", ResultType.Error);
        }
    }

    /// Retrieves all workout sessions belonging to a specific user.
    public async Task<Result<List<WorkoutSession>>> GetUserSessionsAsync(int userId)
    {
        try
        {
            var sessions = await workoutSessionRepository.GetByUserIdAsync(userId);
            return Result<List<WorkoutSession>>.Success(sessions);
        }
        catch (Exception ex)
        {
            return Result<List<WorkoutSession>>.Failure($"Failed to retrieve history: {ex.Message}", ResultType.Error);
        }
    }

    /// Retrieves the currently active session for a user, if one exists.
    public async Task<Result<WorkoutSession>> GetActiveSessionAsync(int userId)
    {
        try
        {
            var session = await workoutSessionRepository.GetActiveSessionByUserIdAsync(userId);
            if (session == null) return Result<WorkoutSession>.NotFound();
            return Result<WorkoutSession>.Success(session);
        }
        catch (Exception ex)
        {
            return Result<WorkoutSession>.Failure($"Failed to check active session: {ex.Message}", ResultType.Error);
        }
    }

    /// Starts a new workout session from a template. 
    /// If an active session exists and cancelExisting is false, it returns a Conflict result.
    public async Task<Result<int>> StartSessionAsync(int userId, int workoutId, bool cancelExisting = false)
    {
        try
        {
            var activeSession = await workoutSessionRepository.GetActiveSessionByUserIdAsync(userId);
            if (activeSession != null)
            {
                if (cancelExisting)
                {
                    activeSession.Status = "cancelled";
                    activeSession.FinishedAt = DateTime.UtcNow;
                    await workoutSessionRepository.UpdateAsync(activeSession);
                }
                else
                {
                    // Signal to the UI that a session is already in progress
                    return Result<int>.Failure("AN_ACTIVE_SESSION_EXISTS", ResultType.Conflict);
                }
            }

            var workout = await workoutRepository.GetByIdAsync(workoutId);
            if (workout == null) return Result<int>.NotFound("Workout template not found.");

            var workoutSession = new WorkoutSession
            {
                UserId = userId,
                WorkoutId = workoutId,
                WorkoutName = workout.Name,
                StartedAt = DateTime.UtcNow,
                Status = "active"
            };

            var sessionId = await workoutSessionRepository.CreateAsync(workoutSession);
            return Result<int>.Success(sessionId);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure($"Failed to start session: {ex.Message}", ResultType.Error);
        }
    }

    /// Updates the status of a session (e.g., to completed or cancelled). 
    /// Automatically sets the finish time and calculates duration if applicable.
    public async Task<Result> UpdateSessionStatusAsync(int sessionId, string status, User editor)
    {
        var workoutSession = await workoutSessionRepository.GetByIdAsync(sessionId);
        
        // Use Centralized Authorization
        var authResult = auth.CanModifySession(editor, workoutSession);
        if (authResult.IsFailure) return authResult;

        workoutSession!.Status = status;
        if (status == "completed" || status == "cancelled")
        {
            workoutSession.FinishedAt = DateTime.UtcNow;
            
            // Basic duration calculation if not already set by UI
            if (workoutSession.TotalSeconds == 0 && workoutSession.FinishedAt.HasValue)
            {
                workoutSession.TotalSeconds = (int)(workoutSession.FinishedAt.Value - workoutSession.StartedAt).TotalSeconds;
            }
        }

        await workoutSessionRepository.UpdateAsync(workoutSession);
        return Result.Success();
    }

    /// Records a new performance set log for an exercise in a session.
    public async Task<Result<WorkoutSetLog>> SaveSetLogAsync(WorkoutSetLog workoutSetLog, User user)
    {
        var workoutSession = await workoutSessionRepository.GetByIdAsync(workoutSetLog.SessionId);
        
        // Use Centralized Authorization
        var authResult = auth.CanModifySession(user, workoutSession);
        if (authResult.IsFailure) return Result<WorkoutSetLog>.Failure(authResult.ErrorMessage!, authResult.Type);

        workoutSetLog.CompletedAt = DateTime.UtcNow;
        var logId = await workoutSessionRepository.AddSetLogAsync(workoutSetLog);
        workoutSetLog.Id = logId;
        return Result<WorkoutSetLog>.Success(workoutSetLog);
    }

    /// Deletes a specific set log from a session.
    public async Task<Result> DeleteSetLogAsync(int sessionId, int logId, User user)
    {
        var workoutSession = await workoutSessionRepository.GetByIdAsync(sessionId);
        
        var authResult = auth.CanModifySession(user, workoutSession);
        if (authResult.IsFailure) return authResult;

        await workoutSessionRepository.DeleteSetLogAsync(logId);
        return Result.Success();
    }

    /// Deletes all logs for a specific exercise in a session.
    public async Task<Result> DeleteExerciseLogsAsync(int sessionId, int exerciseId, User user)
    {
        var workoutSession = await workoutSessionRepository.GetByIdAsync(sessionId);
        
        var authResult = auth.CanModifySession(user, workoutSession);
        if (authResult.IsFailure) return authResult;

        await workoutSessionRepository.DeleteExerciseLogsAsync(sessionId, exerciseId);
        return Result.Success();
    }

    /// Retrieves all sets logged for a specific session.
    public async Task<Result<List<WorkoutSetLog>>> GetSessionLogsAsync(int sessionId)
    {
        try
        {
            var logs = await workoutSessionRepository.GetSetLogsBySessionIdAsync(sessionId);
            return Result<List<WorkoutSetLog>>.Success(logs);
        }
        catch (Exception ex)
        {
            return Result<List<WorkoutSetLog>>.Failure($"Failed to retrieve session logs: {ex.Message}", ResultType.Error);
        }
    }


    /// Deletes a workout session and all associated data.
    public async Task<Result> DeleteSessionAsync(int workoutSessionId, User deleter)
    {
        var workoutSession = await workoutSessionRepository.GetByIdAsync(workoutSessionId);
        
        // Use Centralized Authorization
        var authResult = auth.CanModifySession(deleter, workoutSession);
        if (authResult.IsFailure) return authResult;

        await workoutSessionRepository.DeleteAsync(workoutSessionId);
        return Result.Success();
    }
}
