using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WorkoutTracker.Models;
using WorkoutTracker.UI.Services;

namespace WorkoutTracker.UI.Pages.User;

/// <summary>
/// Page model for managing personal user settings.
/// Handles updating unit preferences and display names.
/// </summary>
public class SettingsModel : PageModel
{
    private readonly PreferenceApiClient _prefApi;

    /// <summary> Gets or sets the user preferences bound to the form. </summary>
    [BindProperty]
    public UserPreferences Preferences { get; set; } = new();

    /// <summary> Gets or sets a status message to display after a save operation. </summary>
    public string? StatusMessage { get; set; }

    /// <summary>
    /// Initializes a new instance of the SettingsModel.
    /// </summary>
    /// <param name="prefApi">Client for preference management.</param>
    public SettingsModel(PreferenceApiClient prefApi)
    {
        _prefApi = prefApi;
    }

    /// <summary>
    /// Lifecycle method executed on GET requests.
    /// Fetches the current preferences for the authenticated user.
    /// </summary>
    /// <returns>A Page result or a redirect to login if unauthenticated.</returns>
    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Auth/Login");

        Preferences = await _prefApi.GetPreferences(userId.Value);
        return Page();
    }

    /// <summary>
    /// Action method for form submission.
    /// Persists the updated preferences via the API client.
    /// </summary>
    /// <returns>The same page with a success message.</returns>
    public async Task<IActionResult> OnPostAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Auth/Login");

        Preferences.UserId = userId.Value;
        await _prefApi.SavePreferences(Preferences, userId.Value);

        StatusMessage = "Preferences updated successfully!";
        return Page();
    }
}
