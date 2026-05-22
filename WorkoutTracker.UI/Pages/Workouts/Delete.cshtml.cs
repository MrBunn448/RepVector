using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WorkoutTracker.Models;
using WorkoutTracker.UI.Services;

namespace WorkoutTracker.UI.Pages.Workouts;

/// <summary>
/// Page model for workout template deletion.
/// Handles confirmation and final removal of workout definitions, including permission verification.
/// </summary>
public class DeleteModel : PageModel
{
    private readonly WorkoutApiClient _api;

    /// <summary> Gets or sets the workout template targeted for deletion. </summary>
    public Workout? Workout { get; set; }

    /// <summary>
    /// Initializes a new instance of the DeleteModel.
    /// </summary>
    /// <param name="api">Client for workout metadata.</param>
    public DeleteModel(WorkoutApiClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Lifecycle method executed on GET requests.
    /// Fetches the workout metadata for the confirmation view and validates ownership.
    /// </summary>
    /// <param name="id">The ID of the workout to delete.</param>
    /// <returns>A Page result, a redirect if unauthenticated, or a 404/403 if not found or unauthorized.</returns>
    public async Task<IActionResult> OnGet(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
            return RedirectToPage("/Auth/Login");

        var isAdmin = HttpContext.Session.GetString("UserRole") == "Admin";
        Workout = await _api.GetWorkoutDetails(id, userId.Value);

        if (Workout == null)
            return NotFound();

        if (Workout.UserId != userId && !isAdmin)
            return Forbid();

        return Page();
    }

    /// <summary>
    /// Action method for deletion confirmation.
    /// Removes the workout definition via the API client.
    /// </summary>
    /// <param name="id">The ID of the workout to delete.</param>
    /// <returns>A redirect to the index page.</returns>
    public async Task<IActionResult> OnPost(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
            return RedirectToPage("/Auth/Login");

        var isAdmin = HttpContext.Session.GetString("UserRole") == "Admin";
        Workout = await _api.GetWorkoutDetails(id, userId.Value);

        if (Workout == null)
            return NotFound();

        if (Workout.UserId != userId && !isAdmin)
            return Forbid();

        await _api.DeleteWorkout(id, userId.Value);
        return RedirectToPage("./Index");
    }
}
