using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WorkoutTracker.Models;
using WorkoutTracker.UI.Services;

namespace WorkoutTracker.UI.Pages.Exercises;

public class DeleteModel : PageModel
{
    private readonly ExerciseApiClient _api;

    public Exercise? Exercise { get; set; }

    public DeleteModel(ExerciseApiClient api)
    {
        _api = api;
    }

    public async Task<IActionResult> OnGet(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
            return RedirectToPage("/Auth/Login");

        var result = await _api.GetExerciseById(id, userId.Value);
        Exercise = result.Value;

        if (result.IsFailure)
            return result.Type == ResultType.NotFound ? NotFound() : RedirectToPage("./Index");

        return Page();
    }

    /// Action method for deletion confirmation.
    /// Removes the exercise record via the API client.
    /// <param name="id">The ID of the exercise to delete.</param>
    /// <returns>A redirect to the index page.</returns>
    public async Task<IActionResult> OnPost(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
            return RedirectToPage("/Auth/Login");

        var result = await _api.GetExerciseById(id, userId.Value);
        Exercise = result.Value;

        if (result.IsFailure)
            return result.Type == ResultType.NotFound ? NotFound() : RedirectToPage("./Index");

        await _api.DeleteExercise(id, userId.Value);
        return RedirectToPage("./Index");
    }
}
