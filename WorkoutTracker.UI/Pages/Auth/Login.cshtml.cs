using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WorkoutTracker.UI.Services;

namespace WorkoutTracker.UI.Pages.Auth;

public class LoginModel : PageModel
{
    private readonly AuthApiClient _authApiClient;

    public LoginModel(AuthApiClient authApiClient)
    {
        _authApiClient = authApiClient;
    }


    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    
    /// Validates credentials via the API and sets up the user session upon success.
    /// returns A redirect to the workouts index if successful, otherwise the same page with an error
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var (success, userId, role, message) = await _authApiClient.LoginAsync(Email, Password);

        if (!success)
        {
            ErrorMessage = message;
            return Page();
        }

        HttpContext.Session.SetInt32("UserId", userId ?? 0);
        HttpContext.Session.SetString("Email", Email);
        HttpContext.Session.SetString("UserRole", role ?? "User");

        return RedirectToPage("/Workouts/Index");
    }
}
