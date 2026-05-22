using Microsoft.AspNetCore.Mvc;
using WorkoutTracker.Logic.Infrastructure;
using WorkoutTracker.Models;

namespace WorkoutTracker.Api.Controllers;

public abstract class BaseWorkoutController(UserContext userContext) : ControllerBase
{
    /// Gets the current user, automatically populated by the UserContextFilter based on the X-User-Id header.
    protected User? CurrentUser => userContext.User;

    protected IActionResult UnauthorizedWithMessage(string message = "User identification required via X-User-Id header.")
    {
        return Unauthorized(new { message });
    }
}
