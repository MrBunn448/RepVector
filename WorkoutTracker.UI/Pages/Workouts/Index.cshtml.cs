using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WorkoutTracker.Models;
using WorkoutTracker.UI.Services;

namespace WorkoutTracker.UI.Pages.Workouts;

public class IndexModel : PageModel
{
    private readonly WorkoutApiClient _api;
    private readonly SessionApiClient _sessionApi;

    public List<Workout> PersonalWorkouts { get; set; } = new();
    public List<Workout> WorkoutTemplates { get; set; } = new();
    public string? ConflictError { get; set; }
    public int? ConflictingWorkoutId { get; set; }

    public IndexModel(WorkoutApiClient api, SessionApiClient sessionApi)
    {
        _api = api;
        _sessionApi = sessionApi;
    }


    /// Fetches all available workouts and categorizes them into personal and predefined.
    public async Task<IActionResult> OnGet()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
            return RedirectToPage("/Auth/Login");

        var allWorkouts = await _api.GetWorkouts(userId.Value);
        
        PersonalWorkouts = allWorkouts.Where(w => !w.IsPredefined).ToList();
        WorkoutTemplates = allWorkouts.Where(w => w.IsPredefined).ToList();

        return Page();
    }

    /// Handles session conflicts by providing an option to override existing sessions.
    public async Task<IActionResult> OnPostStartSessionAsync(int id, bool force = false)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Auth/Login");

        var (success, error) = await _sessionApi.StartSession(id, userId.Value, force);
        if (success)
        {
            return RedirectToPage("./Active");
        }

        if (error == "AN_ACTIVE_SESSION_EXISTS")
        {
            ConflictError = "You already have an ongoing workout session.";
            ConflictingWorkoutId = id;
            return await OnGet();
        }

        return await OnGet();
    }
}