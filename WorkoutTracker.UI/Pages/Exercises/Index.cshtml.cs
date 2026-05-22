using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WorkoutTracker.Models;
using WorkoutTracker.UI.Services;

namespace WorkoutTracker.UI.Pages.Exercises;

/// <summary>
/// Page model for the exercise list index.
/// Displays the full catalog of exercises, categorized by source (global vs. personal).
/// </summary>
public class IndexModel : PageModel
{
    private readonly ExerciseApiClient _api;

    /// <summary> Gets or sets the list of official system exercises. </summary>
    public List<Exercise> RepVectorExercises { get; set; } = new();
    /// <summary> Gets or sets the list of exercises created by the current user. </summary>
    public List<Exercise> PersonalExercises { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of the IndexModel.
    /// </summary>
    /// <param name="api">Client for exercise management.</param>
    public IndexModel(ExerciseApiClient api)
    {
        _api = api;
    }

    /// <summary>
    /// Lifecycle method executed on GET requests.
    /// Fetches all exercises and separates them into global and personal lists.
    /// </summary>
    /// <returns>A Page result or a redirect to login if unauthenticated.</returns>
    public async Task<IActionResult> OnGet()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
            return RedirectToPage("/Auth/Login");

        var allExercises = await _api.GetAllExercises(userId.Value);
        
        RepVectorExercises = allExercises.Where(e => e.IsPredefined).ToList();
        PersonalExercises = allExercises.Where(e => !e.IsPredefined).ToList();

        return Page();
    }
}
