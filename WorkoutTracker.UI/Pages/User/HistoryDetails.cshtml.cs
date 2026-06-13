using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WorkoutTracker.Models;
using WorkoutTracker.UI.Services;

namespace WorkoutTracker.UI.Pages.User;

/// Page model for viewing the granular details of a past workout session.
/// Displays individual sets, weights, and repetitions logged during that specific session.
public class HistoryDetailsModel : PageModel
{
    private readonly SessionApiClient _sessionApi;
    private readonly PreferenceApiClient _prefApi;

    /// Gets or sets the workout session details to display.
    public WorkoutSession? Session { get; set; }
    /// Gets or sets the user preferences for unit display.
    public UserPreferences Preferences { get; set; } = new();

    /// Initializes a new instance of the HistoryDetailsModel.
    public HistoryDetailsModel(SessionApiClient sessionApi, PreferenceApiClient prefApi)
    {
        _sessionApi = sessionApi;
        _prefApi = prefApi;
    }

    public Result<WorkoutSession>? SessionResult { get; set; }

    /// Lifecycle method executed on GET requests.
    /// Fetches the specific session and validates that it belongs to the authenticated user.
    /// <param name="id">The ID of the session to view.</param>
    /// <returns>A Page result, a redirect if unauthenticated, or a 404 if not found or unauthorized.</returns>
    public async Task<IActionResult> OnGetAsync(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Auth/Login");

        SessionResult = await _sessionApi.GetById(id, userId.Value);
        
        if (SessionResult.IsFailure)
            return SessionResult.Type == ResultType.NotFound ? NotFound() : Page();

        Session = SessionResult.Value!;
        if (Session.UserId != userId.Value)
            return NotFound();

        var prefsResult = await _prefApi.GetPreferences(userId.Value);
        Preferences = prefsResult.Value ?? new UserPreferences { UserId = userId.Value };

        return Page();
    }

    /// Action method to delete the session record from history.
    /// <param name="id">The ID of the session to remove.</param>
    /// <returns>A redirect to the user profile index.</returns>
    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Auth/Login");

        var result = await _sessionApi.GetById(id, userId.Value);
        if (result.IsFailure || result.Value?.UserId != userId.Value)
            return NotFound();

        await _sessionApi.DeleteSession(id, userId.Value);
        return RedirectToPage("./Index");
    }
}
