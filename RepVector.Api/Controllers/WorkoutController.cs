using Microsoft.AspNetCore.Mvc;
using RepVector.Logic;

namespace RepVector.Api.Controllers;

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
    public IActionResult GetAll()
    {
        return Ok(_service.GetWorkouts());
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var workout = _service.GetWorkout(id);
        return workout == null ? NotFound() : Ok(workout);
    }
}