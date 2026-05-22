using Microsoft.AspNetCore.Mvc;
using WorkoutTracker.Logic.Services;
using WorkoutTracker.Logic.Infrastructure;
using WorkoutTracker.Models;
using WorkoutTracker.Api.Infrastructure;

namespace WorkoutTracker.Api.Controllers;

/// <summary>
/// Controller for managing the exercises within a workout template.
/// Provides endpoints to link exercises to workouts and manage their targets.
/// </summary>
[ApiController]
[Route("api/workout-exercises")]
public class WorkoutExercisesController(IWorkoutExerciseService service, UserContext userContext) 
    : BaseWorkoutController(userContext)
{
    /// <summary>
    /// Retrieves all exercise associations for a specific workout.
    /// </summary>
    /// <param name="workoutId">The ID of the parent workout template.</param>
    /// <returns>A list of workout-exercise associations.</returns>
    [HttpGet("{workoutId}")]
    public async Task<IActionResult> GetByWorkoutId(int workoutId)
    {
        var result = await service.GetByWorkoutIdAsync(workoutId);
        return Ok(result);
    }

    /// <summary>
    /// Adds a new exercise link to a workout template.
    /// </summary>
    /// <param name="request">The association data including targets and exercise ID.</param>
    /// <returns>A Result mapped to an ActionResult.</returns>
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] WorkoutExercise request)
    {
        if (CurrentUser == null) return UnauthorizedWithMessage();

        // One-liner: Business logic handles the "how", Controller maps the result
        return (await service.AddAsync(request, CurrentUser)).ToActionResult();
    }

    /// <summary>
    /// Updates an existing exercise link within a workout template.
    /// </summary>
    /// <param name="workoutExerciseId">The ID from the URL.</param>
    /// <param name="request">The updated association data.</param>
    /// <returns>A Result mapped to an ActionResult.</returns>
    [HttpPut("{workoutExerciseId}")]
    public async Task<IActionResult> Update(int workoutExerciseId, [FromBody] WorkoutExercise request)
    {
        if (CurrentUser == null) return UnauthorizedWithMessage();

        if (workoutExerciseId != request.Id)
            return BadRequest("ID mismatch");

        return (await service.UpdateAsync(request, CurrentUser)).ToActionResult();
    }

    /// <summary>
    /// Removes an exercise link from a workout template.
    /// </summary>
    /// <param name="workoutExerciseId">The ID of the link to remove.</param>
    /// <returns>A Result mapped to an ActionResult.</returns>
    [HttpDelete("{workoutExerciseId}")]
    public async Task<IActionResult> Delete(int workoutExerciseId)
    {
        if (CurrentUser == null) return UnauthorizedWithMessage();

        return (await service.DeleteAsync(workoutExerciseId, CurrentUser)).ToActionResult();
    }
}