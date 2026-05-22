using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WorkoutTracker.UI.Services;

namespace WorkoutTracker.UI.Pages.Auth;

public class RegisterAdminModel : PageModel
{
    private readonly AuthApiClient _authApiClient;

    public RegisterAdminModel(AuthApiClient authApiClient)
    {
        _authApiClient = authApiClient;
    }

    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    public string ConfirmPassword { get; set; } = string.Empty;

    [BindProperty]
    public string AdminSecret { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public string? SuccessMessage { get; set; }

    public void OnGet()
    {
    }

    /// Validates matching passwords and the admin secret via the API client.
    /// returns A redirect to the login page if successful, otherwise the current page with error state.
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Passwords do not match.";
            return Page();
        }

        var (success, userId, role, message) = await _authApiClient.RegisterAdminAsync(Email, Password, AdminSecret);

        if (!success)
        {
            ErrorMessage = message;
            return Page();
        }

        SuccessMessage = "Admin registration successful! Redirecting to login...";
        return RedirectToPage("/Auth/Login");
    }
}
