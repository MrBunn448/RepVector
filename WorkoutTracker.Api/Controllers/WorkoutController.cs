using Microsoft.AspNetCore.Mvc;
using WorkoutTracker.Logic.Services;

namespace WorkoutTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkoutsController : ControllerBase
{
    private readonly WorkoutService _workoutService;

    public WorkoutsController(WorkoutService workoutService)
    {
        _workoutService = workoutService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var workouts = await _workoutService.GetAllAsync();
        return Ok(workouts);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var workout = await _workoutService.GetWorkoutDetailsAsync(id);

        if (workout == null)
            return NotFound();

        return Ok(workout);
    }
}