using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WorkoutTracker.Models;
using WorkoutTracker.UI.Services;

namespace WorkoutTracker.UI.Pages.Workouts;

public class DetailsModel : PageModel
{
    private readonly WorkoutApiClient _apiClient;

    public DetailsModel(WorkoutApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Workout? Workout { get; private set; }

    public async Task<IActionResult> OnGet(int id)
    {
        Workout = await _apiClient.GetWorkoutDetails(id);

        if (Workout == null)
        {
            return NotFound();
        }

        return Page();
    }
}