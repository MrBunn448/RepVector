using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WorkoutTracker.Models;
using WorkoutTracker.UI.Services;

namespace WorkoutTracker.UI.Pages.Workouts;

public class CreateModel : PageModel
{
    private readonly WorkoutApiClient _api;
    private readonly ExerciseApiClient _exerciseApi;

    [BindProperty]
    public Workout Workout { get; set; } = new();

    public List<Exercise> AllExercises { get; set; } = new();
    public bool IsAdmin { get; set; }
    public string? ErrorMessage { get; set; }

    public CreateModel(WorkoutApiClient api, ExerciseApiClient exerciseApi)
    {
        _api = api;
        _exerciseApi = exerciseApi;
    }

    /// Internal helper to set the IsAdmin flag based on the current session.
    private void SetAdminStatus()
    {
        IsAdmin = HttpContext.Session.GetString("UserRole") == "Admin";
    }

    /// Prepares the view by checking roles and loading exercise options.
    /// A Page result or a redirect to login if unauthenticated.
    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
            return RedirectToPage("/Auth/Login");

        SetAdminStatus();
        AllExercises = (await _exerciseApi.GetAllExercises(userId.Value)).Value ?? new();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
            return RedirectToPage("/Auth/Login");

        SetAdminStatus();
        AllExercises = (await _exerciseApi.GetAllExercises(userId.Value)).Value ?? new();

        if (!ModelState.IsValid)
            return Page();

        try
        {
            if (!IsAdmin)
            {
                Workout.IsPredefined = false;
                Workout.UserId = userId.Value;
            }
            else if (Workout.IsPredefined)
            {
                Workout.UserId = null;
            }
            else
            {
                Workout.UserId = userId.Value;
            }

            Workout.CreatedAt = DateTime.UtcNow;

            await _api.CreateWorkout(Workout, userId.Value);
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to create workout: {ex.Message}";
            return Page();
        }
    }
}
