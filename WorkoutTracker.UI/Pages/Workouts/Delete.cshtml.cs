using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WorkoutTracker.Models;
using WorkoutTracker.UI.Services;

namespace WorkoutTracker.UI.Pages.Workouts;

/// Handles confirmation and final removal of workout definitions, including permission verification.
public class DeleteModel : PageModel
{
    private readonly WorkoutApiClient _api;

    public Workout? Workout { get; set; }

    public DeleteModel(WorkoutApiClient api)
    {
        _api = api;
    }

    /// Fetches the workout metadata for the confirmation view and validates ownership.
    public async Task<IActionResult> OnGet(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
            return RedirectToPage("/Auth/Login");

        var isAdmin = HttpContext.Session.GetString("UserRole") == "Admin";
        var result = await _api.GetWorkoutDetails(id, userId.Value);

        if (result.IsFailure)
            return result.Type == ResultType.NotFound ? NotFound() : RedirectToPage("./Index");

        Workout = result.Value!;

        if (Workout.UserId != userId && !isAdmin)
            return Forbid();

        return Page();
    }

    /// Removes the workout definition via the API client.
    public async Task<IActionResult> OnPost(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
            return RedirectToPage("/Auth/Login");

        var isAdmin = HttpContext.Session.GetString("UserRole") == "Admin";
        var result = await _api.GetWorkoutDetails(id, userId.Value);

        if (result.IsFailure)
            return result.Type == ResultType.NotFound ? NotFound() : RedirectToPage("./Index");

        Workout = result.Value!;

        if (Workout.UserId != userId && !isAdmin)
            return Forbid();

        await _api.DeleteWorkout(id, userId.Value);
        return RedirectToPage("./Index");
    }
}
