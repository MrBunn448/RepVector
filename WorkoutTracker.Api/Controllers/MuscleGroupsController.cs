using Microsoft.AspNetCore.Mvc;
using WorkoutTracker.Logic.Abstractions.Repositories;
using WorkoutTracker.Logic.Infrastructure;
using WorkoutTracker.Models;

namespace WorkoutTracker.Api.Controllers;

/// Provides endpoints to list and manage muscle groups, primarily used by admin's
[ApiController]
[Route("api/muscle-groups")]
public class MuscleGroupsController(IMuscleGroupRepository muscleGroupRepository, UserContext userContext) 
    : BaseWorkoutController(userContext)
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await muscleGroupRepository.GetAllAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] MuscleGroup muscleGroup)
    {
        if (CurrentUser == null || CurrentUser.Role != "Admin") 
            return UnauthorizedWithMessage("Admin access required.");

        var muscleGroupId = await muscleGroupRepository.CreateAsync(muscleGroup);
        return Ok(new { muscleGroupId });
    }

    [HttpDelete("{muscleGroupId}")]
    public async Task<IActionResult> Delete(int muscleGroupId)
    {
        if (CurrentUser == null || CurrentUser.Role != "Admin") 
            return UnauthorizedWithMessage("Admin access required.");

        await muscleGroupRepository.DeleteAsync(muscleGroupId);
        return NoContent();
    }
}
