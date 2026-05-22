using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WorkoutTracker.Models;
using WorkoutTracker.UI.Services;

namespace WorkoutTracker.UI.Pages.User;

/// <summary>
/// Page model for the user profile index.
/// Displays the user's workout session history and general statistics.
/// </summary>
public class IndexModel : PageModel
{
    private readonly SessionApiClient _sessionApi;
    private readonly PreferenceApiClient _prefApi;

    /// <summary> Gets or sets the chronological list of past workout sessions. </summary>
    public List<WorkoutSession> SessionHistory { get; set; } = new();
    /// <summary> Gets or sets the user's preferred measurement units. </summary>
    public UserPreferences Preferences { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of the IndexModel.
    /// </summary>
    public IndexModel(SessionApiClient sessionApi, PreferenceApiClient prefApi)
    {
        _sessionApi = sessionApi;
        _prefApi = prefApi;
    }

    /// <summary>
    /// Lifecycle method executed on GET requests.
    /// Fetches the session history and preferences for the authenticated user.
    /// </summary>
    /// <returns>A Page result or a redirect to login if unauthenticated.</returns>
    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Auth/Login");

        SessionHistory = await _sessionApi.GetHistory(userId.Value);
        Preferences = await _prefApi.GetPreferences(userId.Value);

        return Page();
    }

    /// <summary>
    /// Action method to delete a specific historical session.
    /// </summary>
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
