using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WorkoutTracker.Models;
using WorkoutTracker.UI.Services;

namespace WorkoutTracker.UI.Pages.Workouts;

public class DetailsModel : PageModel
{
    private readonly WorkoutApiClient _api;

    public Workout? Workout { get; set; }

    public DetailsModel(WorkoutApiClient api)
    {
        _api = api;
    }

    public async Task<IActionResult> OnGet(int id)
    {
        Workout = await _api.GetWorkout(id);

        if (Workout == null)
            return NotFound();

        return Page();
    }
}