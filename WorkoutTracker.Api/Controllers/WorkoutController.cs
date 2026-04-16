using Microsoft.AspNetCore.Mvc;
using WorkoutTracker.Logic;

namespace WorkoutTracker.Api.Controllers;

[ApiController]
[Route("api/workouts")]
public class WorkoutController : ControllerBase
{
    private readonly WorkoutService _service;

    public WorkoutController(WorkoutService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllWorkouts());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var workout = await _service.GetWorkout(id);
        if (workout == null) return NotFound();

        return Ok(workout);
    }
}