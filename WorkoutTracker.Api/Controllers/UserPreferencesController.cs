using Microsoft.AspNetCore.Mvc;
using WorkoutTracker.Logic.Services;
using WorkoutTracker.Logic.Infrastructure;
using WorkoutTracker.Models;
using WorkoutTracker.Api.Infrastructure;

namespace WorkoutTracker.Api.Controllers;

[ApiController]
[Route("api/user-preferences")]
public class UserPreferencesController(IPreferenceService preferenceService, UserContext userContext) 
    : BaseWorkoutController(userContext)
{
    /// Retrieves the preferences for the currently authenticated user.
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        if (CurrentUser == null) return UnauthorizedWithMessage();

        var userPreferences = await preferenceService.GetPreferencesAsync(CurrentUser.Id);
        if (userPreferences == null)
        {
            // Return default prefs if none exist
            userPreferences = new UserPreferences { UserId = CurrentUser.Id };
        }
        return Ok(userPreferences);
    }

    /// Saves or updates the preferences for the authenticated user.
    [HttpPost]
    public async Task<IActionResult> Save([FromBody] UserPreferences userPreferences)
    {
        if (CurrentUser == null) return UnauthorizedWithMessage();

        return (await preferenceService.SavePreferencesAsync(userPreferences, CurrentUser)).ToActionResult();
    }
}
