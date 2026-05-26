using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WorkoutTracker.Logic.Abstractions.Repositories;
using WorkoutTracker.Logic.Infrastructure;

namespace WorkoutTracker.Api.Infrastructure;

/// A global action filter that extracts user identification from request headers.
/// It populates a scoped UserContext object by fetching the User from the repository using the X-User-Id header.

public class UserContextFilter : IAsyncActionFilter
{
    private readonly IUserRepository _userRepository;
    private readonly UserContext _userContext;

    public UserContextFilter(IUserRepository userRepository, UserContext userContext)
    {
        _userRepository = userRepository;
        _userContext = userContext;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.HttpContext.Request.Headers.TryGetValue("X-User-Id", out var userIdString) && 
            int.TryParse(userIdString, out var userId))
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user != null)
            {
                _userContext.User = user;
            }
        }
        // Proceed to the controller action
        await next();
    }
}
