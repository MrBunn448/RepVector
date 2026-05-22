using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace WorkoutTracker.UI.Pages
{
    /// <summary>
    /// Page model for the application error page.
    /// Provides diagnostic information about the request that caused the error.
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {
        /// <summary> Gets or sets the unique ID of the request. </summary>
        public string? RequestId { get; set; }

        /// <summary> Gets a value indicating whether the request ID should be displayed. </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        /// <summary>
        /// Lifecycle method executed on GET requests.
        /// Captures the current activity ID or trace identifier.
        /// </summary>
        public void OnGet()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }
    }
}
