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
    public async Task<WorkoutSession?> GetByIdAsync(int workoutSessionId)
    {
        return await workoutSessionRepository.GetByIdAsync(workoutSessionId);
    }

    /// Retrieves all workout sessions belonging to a specific user.
    public async Task<List<WorkoutSession>> GetUserSessionsAsync(int userId)
    {
        return await workoutSessionRepository.GetByUserIdAsync(userId);
    }

    /// Retrieves the currently active session for a user, if one exists.
    public async Task<WorkoutSession?> GetActiveSessionAsync(int userId)
    {
        return await workoutSessionRepository.GetActiveSessionByUserIdAsync(userId);
    }

    /// Starts a new workout session from a template. 
    /// If an active session exists and cancelExisting is false, it returns a Conflict result.
    public async Task<Result<int>> StartSessionAsync(int userId, int workoutId, bool cancelExisting = false)
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
    public async Task<Result> SaveSetLogAsync(WorkoutSetLog workoutSetLog, User user)
    {
        var workoutSession = await workoutSessionRepository.GetByIdAsync(workoutSetLog.SessionId);
        
        // Use Centralized Authorization
        var authResult = auth.CanModifySession(user, workoutSession);
        if (authResult.IsFailure) return authResult;

        workoutSetLog.CompletedAt = DateTime.UtcNow;
        await workoutSessionRepository.AddSetLogAsync(workoutSetLog);
        return Result.Success();
    }

    /// Retrieves all sets logged for a specific session.
    public async Task<List<WorkoutSetLog>> GetSessionLogsAsync(int sessionId)
    {
        return await workoutSessionRepository.GetSetLogsBySessionIdAsync(sessionId);
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
