using Microsoft.AspNetCore.Mvc;
using WorkoutTracker.Logic.Abstractions.Services;
using WorkoutTracker.Logic.Infrastructure;
using WorkoutTracker.Models;
using WorkoutTracker.Api.Infrastructure;

namespace WorkoutTracker.Api.Controllers;

/// Provides endpoints to list and manage muscle groups, primarily used by admin's
[ApiController]
[Route("api/muscle-groups")]
public class MuscleGroupsController(IMetadataService metadataService, UserContext userContext) 
    : BaseWorkoutController(userContext)
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await metadataService.GetAllMuscleGroupsAsync();
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] MuscleGroup muscleGroup)
    {
        if (CurrentUser == null || CurrentUser.Role != "Admin") 
            return UnauthorizedWithMessage("Admin access required.");

        var result = await metadataService.CreateMuscleGroupAsync(muscleGroup);
        if (result.IsSuccess) return Ok(new { muscleGroupId = result.Value });
        return result.ToActionResult();
    }

    [HttpDelete("{muscleGroupId}")]
    public async Task<IActionResult> Delete(int muscleGroupId)
    {
        if (CurrentUser == null || CurrentUser.Role != "Admin") 
            return UnauthorizedWithMessage("Admin access required.");

        return (await metadataService.DeleteMuscleGroupAsync(muscleGroupId)).ToActionResult();
    }
}
