using Microsoft.AspNetCore.Mvc;
using WorkoutTracker.Logic.Services;
using WorkoutTracker.Logic.Infrastructure;
using WorkoutTracker.Models;
using WorkoutTracker.Api.Infrastructure;

namespace WorkoutTracker.Api.Controllers;

/// Provides endpoints for CRUD operations on exercises with role-based access control.
[ApiController]
[Route("api/[controller]")]
public class ExercisesController(IExerciseService exerciseService, UserContext userContext) 
    : BaseWorkoutController(userContext)
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (CurrentUser == null) return UnauthorizedWithMessage();

        var result = await exerciseService.GetAllAsync(CurrentUser.Id);
        return result.ToActionResult();
    }

    [HttpGet("{exerciseId}")]
    public async Task<IActionResult> GetById(int exerciseId)
    {
        if (CurrentUser == null) return UnauthorizedWithMessage();

        var result = await exerciseService.GetByIdAsync(exerciseId);

        if (result.IsFailure) return result.ToActionResult();

        var exercise = result.Value;

        // View logic: check if owner or predefined
        if (exercise!.UserId != CurrentUser.Id && !exercise.IsPredefined)
            return Forbid("You do not have permission to view this exercise.");

        return Ok(exercise);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Exercise exercise)
    {
        if (CurrentUser == null) return UnauthorizedWithMessage();

        if (string.IsNullOrWhiteSpace(exercise.Name))
            return BadRequest("Exercise name is required.");

        var result = await exerciseService.CreateAsync(exercise, CurrentUser);
        
        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetById), new { exerciseId = result.Value }, new { id = result.Value });
        }

        return result.ToActionResult();
    }

    [HttpPut("{exerciseId}")]
    public async Task<IActionResult> Update(int exerciseId, [FromBody] Exercise exercise)
    {
        if (CurrentUser == null) return UnauthorizedWithMessage();

        if (exerciseId != exercise.Id)
            return BadRequest("ID mismatch.");

        return (await exerciseService.UpdateAsync(exercise, CurrentUser)).ToActionResult();
    }

    [HttpDelete("{exerciseId}")]
    public async Task<IActionResult> Delete(int exerciseId)
    {
        if (CurrentUser == null) return UnauthorizedWithMessage();

        return (await exerciseService.DeleteAsync(exerciseId, CurrentUser)).ToActionResult();
    }
}
