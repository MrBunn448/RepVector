using Microsoft.AspNetCore.Mvc;
using WorkoutTracker.Logic.Services;
using WorkoutTracker.Logic.Infrastructure;
using WorkoutTracker.Models;
using WorkoutTracker.Api.Infrastructure;

namespace WorkoutTracker.Api.Controllers;


/// Controller for managing the exercises within a workout template.
/// Provides endpoints to link exercises to workouts and manage their targets.
[ApiController]
[Route("api/workout-exercises")]
public class WorkoutExercisesController(IWorkoutExerciseService service, UserContext userContext) 
    : BaseWorkoutController(userContext)
{

    /// Retrieves all exercise associations for a specific workout.
    [HttpGet("{workoutId}")]
    public async Task<IActionResult> GetByWorkoutId(int workoutId)
    {
        var result = await service.GetByWorkoutIdAsync(workoutId);
        return Ok(result);
    }

    /// Adds a new exercise link to a workout template.
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] WorkoutExercise request)
    {
        if (CurrentUser == null) return UnauthorizedWithMessage();

        // One-liner: Business logic handles the "how", Controller maps the result
        return (await service.AddAsync(request, CurrentUser)).ToActionResult();
    }

    /// Updates an existing exercise link within a workout template.
    [HttpPut("{workoutExerciseId}")]
    public async Task<IActionResult> Update(int workoutExerciseId, [FromBody] WorkoutExercise request)
    {
        if (CurrentUser == null) return UnauthorizedWithMessage();

        if (workoutExerciseId != request.Id)
            return BadRequest("ID mismatch");

        return (await service.UpdateAsync(request, CurrentUser)).ToActionResult();
    }

    /// Removes an exercise link from a workout template.
    [HttpDelete("{workoutExerciseId}")]
    public async Task<IActionResult> Delete(int workoutExerciseId)
    {
        if (CurrentUser == null) return UnauthorizedWithMessage();

        return (await service.DeleteAsync(workoutExerciseId, CurrentUser)).ToActionResult();
    }
}