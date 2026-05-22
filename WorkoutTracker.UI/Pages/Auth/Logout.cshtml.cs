using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WorkoutTracker.UI.Pages.Auth;

public class LogoutModel : PageModel
{

    /// On load executes
    public IActionResult OnGet()
    {
        HttpContext.Session.Clear();
        
        // Ensure the browser doesn't cache the logout page (Note: This was to fix the "Back" button issue)
        Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
        Response.Headers.Append("Pragma", "no-cache");
        Response.Headers.Append("Expires", "0");

        return RedirectToPage("/Auth/Login");
    }
}
