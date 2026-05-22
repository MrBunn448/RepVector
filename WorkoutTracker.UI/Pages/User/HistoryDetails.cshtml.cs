using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WorkoutTracker.Models;
using WorkoutTracker.UI.Services;

namespace WorkoutTracker.UI.Pages.User;

/// <summary>
/// Page model for viewing the granular details of a past workout session.
/// Displays individual sets, weights, and repetitions logged during that specific session.
/// </summary>
public class HistoryDetailsModel : PageModel
{
    private readonly SessionApiClient _sessionApi;
    private readonly PreferenceApiClient _prefApi;

    /// <summary> Gets or sets the workout session details to display. </summary>
    public WorkoutSession? Session { get; set; }
    /// <summary> Gets or sets the user preferences for unit display. </summary>
    public UserPreferences Preferences { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of the HistoryDetailsModel.
    /// </summary>
    public HistoryDetailsModel(SessionApiClient sessionApi, PreferenceApiClient prefApi)
    {
        _sessionApi = sessionApi;
        _prefApi = prefApi;
    }

    /// <summary>
    /// Lifecycle method executed on GET requests.
    /// Fetches the specific session and validates that it belongs to the authenticated user.
    /// </summary>
    /// <param name="id">The ID of the session to view.</param>
    /// <returns>A Page result, a redirect if unauthenticated, or a 404 if not found or unauthorized.</returns>
    public async Task<IActionResult> OnGetAsync(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Auth/Login");

        Session = await _sessionApi.GetById(id, userId.Value);
        if (Session == null || Session.UserId != userId.Value)
            return NotFound();

        Preferences = await _prefApi.GetPreferences(userId.Value);

        return Page();
    }

    /// <summary>
    /// Action method to delete the session record from history.
    /// </summary>
    /// <param name="id">The ID of the session to remove.</param>
    /// <returns>A redirect to the user profile index.</returns>
    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Auth/Login");

        var session = await _sessionApi.GetById(id, userId.Value);
        if (session == null || session.UserId != userId.Value)
            return NotFound();

        await _sessionApi.DeleteSession(id, userId.Value);
        return RedirectToPage("./Index");
    }
}
