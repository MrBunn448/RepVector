using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WorkoutTracker.Models;
using WorkoutTracker.UI.Services;

namespace WorkoutTracker.UI.Pages.User;

/// Page model for the user profile index.
/// Displays the user's workout session history and general statistics.
public class IndexModel : PageModel
{
    private readonly SessionApiClient _sessionApi;
    private readonly PreferenceApiClient _prefApi;

    /// Gets or sets the chronological list of past workout sessions.
    public List<WorkoutSession> SessionHistory { get; set; } = new();
    /// Gets or sets the user's preferred measurement units.
    public UserPreferences Preferences { get; set; } = new();

    /// Initializes a new instance of the IndexModel.
    public IndexModel(SessionApiClient sessionApi, PreferenceApiClient prefApi)
    {
        _sessionApi = sessionApi;
        _prefApi = prefApi;
    }

    public Result<List<WorkoutSession>>? HistoryResult { get; set; }

    /// Lifecycle method executed on GET requests.
    /// Fetches the session history and preferences for the authenticated user.
    /// <returns>A Page result or a redirect to login if unauthenticated.</returns>
    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Auth/Login");

        HistoryResult = await _sessionApi.GetHistory(userId.Value);
        if (HistoryResult.IsSuccess)
        {
            SessionHistory = HistoryResult.Value ?? new();
        }

        var prefsResult = await _prefApi.GetPreferences(userId.Value);
        Preferences = prefsResult.Value ?? new UserPreferences { UserId = userId.Value };

        return Page();
    }

    /// Action method to delete a specific historical session.
    /// <param name="id">The ID of the session to remove.</param>
    /// <returns>A redirect back to the profile page.</returns>
    public async Task<IActionResult> OnPostDeleteSessionAsync(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Auth/Login");

        await _sessionApi.DeleteSession(id, userId.Value);
        return RedirectToPage();
    }
}
