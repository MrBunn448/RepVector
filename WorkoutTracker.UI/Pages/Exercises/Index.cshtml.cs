using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WorkoutTracker.Models;
using WorkoutTracker.UI.Services;

namespace WorkoutTracker.UI.Pages.Exercises;

/// Page model for the exercise list index.
/// Displays the full catalog of exercises, categorized by source (global vs. personal).
public class IndexModel : PageModel
{
    private readonly ExerciseApiClient _api;

    /// Gets or sets the list of official system exercises.
    public List<Exercise> RepVectorExercises { get; set; } = new();
    /// Gets or sets the list of exercises created by the current user.
    public List<Exercise> PersonalExercises { get; set; } = new();

    /// Initializes a new instance of the IndexModel.
    /// <param name="api">Client for exercise management.</param>
    public IndexModel(ExerciseApiClient api)
    {
        _api = api;
    }

    /// Gets or sets the result of the exercise fetch operation.
    public Result<List<Exercise>>? ExerciseResult { get; set; }

    /// Lifecycle method executed on GET requests.
    /// Fetches all exercises and separates them into global and personal lists.
    /// <returns>A Page result or a redirect to login if unauthenticated.</returns>
    public async Task<IActionResult> OnGet()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
            return RedirectToPage("/Auth/Login");

        ExerciseResult = await _api.GetAllExercises(userId.Value);
        
        if (ExerciseResult.IsSuccess)
        {
            RepVectorExercises = ExerciseResult.Value!.Where(e => e.IsPredefined).ToList();
            PersonalExercises = ExerciseResult.Value!.Where(e => !e.IsPredefined).ToList();
        }

        return Page();
    }
}
