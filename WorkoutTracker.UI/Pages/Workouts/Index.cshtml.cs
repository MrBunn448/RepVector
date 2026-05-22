using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WorkoutTracker.Models;
using WorkoutTracker.UI.Services;

namespace WorkoutTracker.UI.Pages.Workouts;

/// <summary>
/// Page model for the workout list index.
/// Handles displaying personal and global workout templates and initiating new workout sessions.
/// </summary>
public class IndexModel : PageModel
{
    private readonly WorkoutApiClient _api;
    private readonly SessionApiClient _sessionApi;

    /// <summary> Gets or sets the list of workouts created by the user. </summary>
    public List<Workout> PersonalWorkouts { get; set; } = new();
    /// <summary> Gets or sets the list of official global workout templates. </summary>
    public List<Workout> WorkoutTemplates { get; set; } = new();
    /// <summary> Gets or sets an error message to display if a session conflict occurs. </summary>
    public string? ConflictError { get; set; }
    /// <summary> Gets or sets the ID of the workout that was attempted to be started during a conflict. </summary>
    public int? ConflictingWorkoutId { get; set; }

    /// <summary>
    /// Initializes a new instance of the IndexModel.
    /// </summary>
    /// <param name="api">Client for workout metadata.</param>
    /// <param name="sessionApi">Client for session management.</param>
    public IndexModel(WorkoutApiClient api, SessionApiClient sessionApi)
    {
        _api = api;
        _sessionApi = sessionApi;
    }

    /// <summary>
    /// Lifecycle method executed on GET requests.
    /// Fetches all available workouts and categorizes them into personal and predefined.
    /// </summary>
    /// <returns>A Page result or a redirect to login if unauthenticated.</returns>
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

    /// <summary>
    /// Action method to start a new workout session based on a template.
    /// Handles session conflicts by providing an option to override existing sessions.
    /// </summary>
    /// <param name="id">The ID of the workout template to start.</param>
    /// <param name="force">If true, cancels any existing active session before starting.</param>
    /// <returns>A redirect to the active session page or the current page with error state.</returns>
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