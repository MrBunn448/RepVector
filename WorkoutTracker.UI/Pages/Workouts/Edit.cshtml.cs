using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WorkoutTracker.Models;
using WorkoutTracker.UI.Services;

namespace WorkoutTracker.UI.Pages.Workouts;

public class EditModel : PageModel
{
    private readonly WorkoutApiClient _api;
    private readonly ExerciseApiClient _exerciseApi;

    [BindProperty]
    public Workout Workout { get; set; } = new();

    public List<Exercise> AllExercises { get; set; } = new();

    public bool IsAdmin { get; set; }

    public EditModel(WorkoutApiClient api, ExerciseApiClient exerciseApi)
    {
        _api = api;
        _exerciseApi = exerciseApi;
    }


    /// Loads the workout details and validates that the user has permission to edit it.
    public async Task<IActionResult> OnGet(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
            return RedirectToPage("/Auth/Login");

        IsAdmin = HttpContext.Session.GetString("UserRole") == "Admin";
        Workout = await _api.GetWorkoutDetails(id, userId.Value);

        if (Workout == null)
            return NotFound();

        // Permission check
        if (Workout.UserId != userId && !IsAdmin)
            return Forbid();

        AllExercises = await _exerciseApi.GetAllExercises(userId.Value);

        return Page();
    }

    /// Validates ownership logic, ensures data consistency, and updates the workout record.
    public async Task<IActionResult> OnPost()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
            return RedirectToPage("/Auth/Login");

        IsAdmin = HttpContext.Session.GetString("UserRole") == "Admin";

        // Re-load exercises in case of validation error or to use in page
        AllExercises = await _exerciseApi.GetAllExercises(userId.Value);

        if (!ModelState.IsValid)
            return Page();

        // Permission check (existing workout check)
        var existing = await _api.GetWorkoutDetails(Workout.Id, userId.Value);
        if (existing == null) return NotFound();
        
        // Block normal users from editing global templates or others' workouts
        if (existing.IsPredefined && !IsAdmin)
            return Forbid();
        
        if (!existing.IsPredefined && existing.UserId != userId && !IsAdmin)
            return Forbid();

        // If admin is turning a regular workout into a template or vice versa
        if (IsAdmin)
        {
            if (Workout.IsPredefined)
            {
                Workout.UserId = null;
            }
            else if (Workout.UserId == null)
            {
                Workout.UserId = userId.Value;
            }
        }
        else
        {
            // Normal user can only save as their own and NOT predefined
            Workout.UserId = userId.Value;
            Workout.IsPredefined = false;
        }

        await _api.UpdateWorkout(Workout, userId.Value);
        return RedirectToPage("./Index");
    }
}
