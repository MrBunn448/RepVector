using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WorkoutTracker.UI.Services;

namespace WorkoutTracker.UI.Pages.Auth;

public class RegisterModel : PageModel
{
    private readonly AuthApiClient _authApiClient;

 
    /// Initializes a new instance of the RegisterModel.
    public RegisterModel(AuthApiClient authApiClient)
    {
        _authApiClient = authApiClient;
    }

    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    public string ConfirmPassword { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public string? SuccessMessage { get; set; }


    /// Action method for registration form submission.
    /// Validates matching passwords and calls the API to create the user account.

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Passwords do not match.";
            return Page();
        }

        var (success, userId, message) = await _authApiClient.RegisterAsync(Email, Password);

        if (!success)
        {
            ErrorMessage = message;
            return Page();
        }

        SuccessMessage = "Registration successful! Redirecting to login...";
        return RedirectToPage("/Auth/Login");
    }
}
