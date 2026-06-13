using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WorkoutTracker.Models;
using WorkoutTracker.UI.Services;

namespace WorkoutTracker.UI.Pages.User;

/// Page model for managing personal user settings.
/// Handles updating unit preferences and display names.
public class SettingsModel : PageModel
{
    private readonly PreferenceApiClient _prefApi;

    /// Gets or sets the user preferences bound to the form.
    [BindProperty]
    public UserPreferences Preferences { get; set; } = new();

    /// Gets or sets a status message to display after a save operation.
    public string? StatusMessage { get; set; }

    /// Initializes a new instance of the SettingsModel.
    /// <param name="prefApi">Client for preference management.</param>
    public SettingsModel(PreferenceApiClient prefApi)
    {
        _prefApi = prefApi;
    }

    /// Lifecycle method executed on GET requests.
    /// Fetches the current preferences for the authenticated user.
    /// <returns>A Page result or a redirect to login if unauthenticated.</returns>
    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Auth/Login");

        var result = await _prefApi.GetPreferences(userId.Value);
        
        if (result.IsSuccess)
        {
            Preferences = result.Value!;
        }
        else if (result.Type == ResultType.NotFound)
        {
            Preferences = new UserPreferences { UserId = userId.Value };
        }
        else
        {
            ModelState.AddModelError("", result.ErrorMessage ?? "Could not load settings.");
            Preferences = new UserPreferences { UserId = userId.Value };
        }

        return Page();
    }

    /// Action method for form submission.
    /// Persists the updated preferences via the API client.
    /// <returns>The same page with a success message.</returns>
    public async Task<IActionResult> OnPostAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Auth/Login");

        Preferences.UserId = userId.Value;

        if (!ModelState.IsValid) return Page();

        var result = await _prefApi.SavePreferences(Preferences, userId.Value);
        
        if (result.IsSuccess)
        {
            StatusMessage = "Preferences updated successfully!";
            return Page();
        }

        ModelState.AddModelError("", result.ErrorMessage ?? "Failed to save settings.");
        return Page();
    }
}
