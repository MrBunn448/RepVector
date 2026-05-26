using Microsoft.AspNetCore.Mvc;
using WorkoutTracker.Logic.Services;
using WorkoutTracker.Logic.Infrastructure;
using WorkoutTracker.Models;
using WorkoutTracker.Api.Infrastructure;

namespace WorkoutTracker.Api.Controllers;

/// Controller for managing live workout sessions.
[ApiController]
[Route("api/[controller]")]
public class WorkoutSessionsController(IWorkoutSessionService sessionService, UserContext userContext) 
    : BaseWorkoutController(userContext)
{
    /// Retrieves a specific workout session by its identifier.
    /// Includes performance logs for the session.
    [HttpGet("{workoutSessionId}")]
    public async Task<IActionResult> GetById(int workoutSessionId)
    {
        if (CurrentUser == null) return UnauthorizedWithMessage();

        var session = await sessionService.GetByIdAsync(workoutSessionId);
        if (session == null) return NotFound();

        // View logic: check if owner or admin
        if (session.UserId != CurrentUser.Id && CurrentUser.Role != "Admin")
            return Forbid();

        session.SetLogs = await sessionService.GetSessionLogsAsync(workoutSessionId);
        return Ok(session);
    }

    /// Retrieves the currently active session for the authenticated user.
    [HttpGet("user/active")]
    public async Task<IActionResult> GetActiveSession()
    {
        if (CurrentUser == null) return UnauthorizedWithMessage();

        var session = await sessionService.GetActiveSessionAsync(CurrentUser.Id);
        if (session == null) return NotFound();

        session.SetLogs = await sessionService.GetSessionLogsAsync(session.Id);
        return Ok(session);
    }

    /// Retrieves the full workout history for the authenticated user.
    [HttpGet("user/history")]
    public async Task<IActionResult> GetHistory()
    {
        if (CurrentUser == null) return UnauthorizedWithMessage();

        var sessions = await sessionService.GetUserSessionsAsync(CurrentUser.Id);
        return Ok(sessions);
    }

    /// Initializes a new workout session based on a workout template.
    [HttpPost("start/{workoutId}")]
    public async Task<IActionResult> StartSession(int workoutId, [FromQuery] bool cancelExisting = false)
    {
        if (CurrentUser == null) return UnauthorizedWithMessage();

        var result = await sessionService.StartSessionAsync(CurrentUser.Id, workoutId, cancelExisting);
        
        if (result.IsSuccess)
        {
            return Ok(new { id = result.Value });
        }

        return result.ToActionResult();
    }

    /// Updates the status of an ongoing session (e.g., to completed or cancelled).
    [HttpPut("{workoutSessionId}/status")]
    public async Task<IActionResult> UpdateStatus(int workoutSessionId, [FromBody] string status)
    {
        if (CurrentUser == null) return UnauthorizedWithMessage();

        return (await sessionService.UpdateSessionStatusAsync(workoutSessionId, status, CurrentUser)).ToActionResult();
    }

    /// Records a new performance set log for an exercise in a session.
    [HttpPost("{workoutSessionId}/log-set")]
    public async Task<IActionResult> LogSet(int workoutSessionId, [FromBody] WorkoutSetLog log)
    {
        if (CurrentUser == null) return UnauthorizedWithMessage();

        log.SessionId = workoutSessionId;
        return (await sessionService.SaveSetLogAsync(log, CurrentUser)).ToActionResult();
    }

    /// Deletes a workout session record.
    [HttpDelete("{workoutSessionId}")]
    public async Task<IActionResult> DeleteSession(int workoutSessionId)
    {
        if (CurrentUser == null) return UnauthorizedWithMessage();

        return (await sessionService.DeleteSessionAsync(workoutSessionId, CurrentUser)).ToActionResult();
    }
}
