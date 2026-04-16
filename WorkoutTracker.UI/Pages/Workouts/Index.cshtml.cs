using Microsoft.AspNetCore.Mvc.RazorPages;
using WorkoutTracker.Models;
using WorkoutTracker.UI.Services;

namespace WorkoutTracker.UI.Pages.Workouts;

public class IndexModel : PageModel
{
    private readonly WorkoutApiClient _api;

    public List<Workout> Workouts { get; set; } = new();

    public IndexModel(WorkoutApiClient api)
    {
        _api = api;
    }

    public async Task OnGet()
    {
        Workouts = await _api.GetWorkouts();
    }
}