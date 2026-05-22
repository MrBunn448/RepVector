using Microsoft.AspNetCore.Mvc;
using WorkoutTracker.Logic.Services;
using WorkoutTracker.Logic.Infrastructure;
using WorkoutTracker.Models;
using WorkoutTracker.Api.Infrastructure;

namespace WorkoutTracker.Api.Controllers;

/// <summary>
/// Controller for managing live workout sessions.
/// Provides endpoints to start sessions, update status, and log performance data.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class WorkoutSessionsController(IWorkoutSessionService sessionService, UserContext userContext) 
    : BaseWorkoutController(userContext)
{
    /// <summary>
    /// Retrieves a specific workout session by its identifier.
    /// Includes performance logs for the session.
    /// </summary>
    /// <param name="workoutSessionId">The unique ID of the session.</param>
    /// <returns>The session details and its set logs.</returns>
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

    /// <summary>
    /// Retrieves the currently active session for the authenticated user.
    /// </summary>
    /// <returns>The active session details if found, otherwise 404.</returns>
    [HttpGet("user/active")]
    public async Task<IActionResult> GetActiveSession()
    {
        if (CurrentUser == null) return UnauthorizedWithMessage();

        var session = await sessionService.GetActiveSessionAsync(CurrentUser.Id);
        if (session == null) return NotFound();

        session.SetLogs = await sessionService.GetSessionLogsAsync(session.Id);
        return Ok(session);
    }

    /// <summary>
    /// Retrieves the full workout history for the authenticated user.
    /// </summary>
    /// <returns>A list of past and current sessions.</returns>
    [HttpGet("user/history")]
    public async Task<IActionResult> GetHistory()
    {
        if (CurrentUser == null) return UnauthorizedWithMessage();

        var sessions = await sessionService.GetUserSessionsAsync(CurrentUser.Id);
        return Ok(sessions);
    }

    /// <summary>
    /// Initializes a new workout session based on a workout template.
    /// </summary>
    /// <param name="workoutId">The ID of the template to start.</param>
    /// <param name="cancelExisting">Whether to cancel any current active session.</param>
    /// <returns>An OK response with the new session ID.</returns>
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

    /// <summary>
    /// Updates the status of an ongoing session (e.g., to completed or cancelled).
    /// </summary>
    /// <param name="workoutSessionId">The ID of the session.</param>
    /// <param name="status">The new status string.</param>
    /// <returns>A Result mapped to an ActionResult.</returns>
    [HttpPut("{workoutSessionId}/status")]
    public async Task<IActionResult> UpdateStatus(int workoutSessionId, [FromBody] string status)
    {
        if (CurrentUser == null) return UnauthorizedWithMessage();

        return (await sessionService.UpdateSessionStatusAsync(workoutSessionId, status, CurrentUser)).ToActionResult();
    }

    /// <summary>
    /// Records a new performance set log for an exercise in a session.
    /// </summary>
    /// <param name="workoutSessionId">The ID of the session.</param>
    /// <param name="log">The set performance data.</param>
    /// <returns>A Result mapped to an ActionResult.</returns>
    [HttpPost("{workoutSessionId}/log-set")]
    public async Task<IActionResult> LogSet(int workoutSessionId, [FromBody] WorkoutSetLog log)
    {
        if (CurrentUser == null) return UnauthorizedWithMessage();

        log.SessionId = workoutSessionId;
        return (await sessionService.SaveSetLogAsync(log, CurrentUser)).ToActionResult();
    }

    /// <summary>
    /// Deletes a workout session record.
    /// </summary>
    /// <param name="workoutSessionId">The ID of the session to delete.</param>
    /// <returns>A Result mapped to an ActionResult.</returns>
    [HttpDelete("{workoutSessionId}")]
    public async Task<IActionResult> DeleteSession(int workoutSessionId)
    {
        if (CurrentUser == null) return UnauthorizedWithMessage();

        return (await sessionService.DeleteSessionAsync(workoutSessionId, CurrentUser)).ToActionResult();
    }
}
