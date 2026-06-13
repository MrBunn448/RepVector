using Microsoft.AspNetCore.Mvc;
using WorkoutTracker.Logic.Services;
using WorkoutTracker.Logic.Infrastructure;
using WorkoutTracker.Models;
using WorkoutTracker.Api.Infrastructure;

namespace WorkoutTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkoutsController(IWorkoutService workoutService, UserContext userContext) 
    : BaseWorkoutController(userContext)
{
    /// Retrieves all workout templates accessible to the current user.
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        if (CurrentUser == null) return UnauthorizedWithMessage();
        var result = await workoutService.GetAllByUserIdAsync(CurrentUser.Id);
        return result.ToActionResult();
    }

    /// Retrieves all workout templates owned by a specific user.
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser(int userId)
    {
        var result = await workoutService.GetAllByUserIdAsync(userId);
        return result.ToActionResult();
    }

    /// Retrieves basic metadata for a specific workout template.
    [HttpGet("{workoutId}")]
    public async Task<IActionResult> GetById(int workoutId)
    {
        if (CurrentUser == null) return UnauthorizedWithMessage();

        var result = await workoutService.GetByIdAsync(workoutId);

        if (result.IsFailure) return result.ToActionResult();

        var workout = result.Value;

        // View logic: check if owner or predefined
        if (workout!.UserId != CurrentUser.Id && !workout.IsPredefined)
            return Forbid("You do not have permission to view this workout.");

        return Ok(workout);
    }

    /// Retrieves a workout template including its full list of exercises.
    [HttpGet("{workoutId}/details")]
    public async Task<IActionResult> GetDetails(int workoutId)
    {
        if (CurrentUser == null) return UnauthorizedWithMessage();

        var result = await workoutService.GetWorkoutDetailsAsync(workoutId);

        if (result.IsFailure) return result.ToActionResult();

        var workout = result.Value;

        if (workout!.UserId != CurrentUser.Id && !workout.IsPredefined)
            return Forbid("You do not have permission to view this workout.");

        return Ok(workout);
    }

    /// Creates a new workout template definition.
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Workout workout)
    {
        if (CurrentUser == null) return UnauthorizedWithMessage();
        
        var result = await workoutService.CreateWorkoutAsync(workout, CurrentUser);
        
        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetById), new { workoutId = result.Value }, new { id = result.Value });
        }

        return result.ToActionResult();
    }

    /// Updates an existing workout template. Validates ownership and ID consistency.
    [HttpPut("{workoutId}")]
    public async Task<IActionResult> Update(int workoutId, [FromBody] Workout workout)
    {
        if (CurrentUser == null) return UnauthorizedWithMessage();

        if (workoutId != workout.Id)
            return BadRequest("ID mismatch.");

        return (await workoutService.UpdateWorkoutAsync(workout, CurrentUser)).ToActionResult();
    }

    /// Deletes a workout template definition.
    [HttpDelete("{workoutId}")]
    public async Task<IActionResult> Delete(int workoutId)
    {
        if (CurrentUser == null) return UnauthorizedWithMessage();

        return (await workoutService.DeleteWorkoutAsync(workoutId, CurrentUser)).ToActionResult();
    }
}
