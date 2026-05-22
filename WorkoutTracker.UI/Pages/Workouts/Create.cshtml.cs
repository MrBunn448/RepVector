using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WorkoutTracker.Models;
using WorkoutTracker.UI.Services;

namespace WorkoutTracker.UI.Pages.Workouts;

/// <summary>
/// Page model for creating a new workout template.
/// Handles the definition of workout metadata and ensures correct ownership assignment based on user role.
/// </summary>
public class CreateModel : PageModel
{
    private readonly WorkoutApiClient _api;
    private readonly ExerciseApiClient _exerciseApi;

    /// <summary> Gets or sets the workout being created. </summary>
    [BindProperty]
    public Workout Workout { get; set; } = new();

    /// <summary> Gets or sets the list of all exercises available to include in the workout. </summary>
    public List<Exercise> AllExercises { get; set; } = new();

    /// <summary> Gets or sets a value indicating whether the current user is an administrator. </summary>
    public bool IsAdmin { get; set; }
    /// <summary> Gets or sets an error message if the creation process fails. </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Initializes a new instance of the CreateModel.
    /// </summary>
    /// <param name="api">Client for workout metadata.</param>
    /// <param name="exerciseApi">Client for exercise metadata.</param>
    public CreateModel(WorkoutApiClient api, ExerciseApiClient exerciseApi)
    {
        _api = api;
        _exerciseApi = exerciseApi;
    }

    /// <summary>
    /// Internal helper to set the IsAdmin flag based on the current session.
    /// </summary>
    private void SetAdminStatus()
    {
        IsAdmin = HttpContext.Session.GetString("UserRole") == "Admin";
    }

    /// <summary>
    /// Lifecycle method executed on GET requests.
    /// Prepares the view by checking roles and loading exercise options.
    /// </summary>
    /// <returns>A Page result or a redirect to login if unauthenticated.</returns>
    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
            return RedirectToPage("/Auth/Login");

        SetAdminStatus();
        AllExercises = await _exerciseApi.GetAllExercises(userId.Value);

        return Page();
    }

    /// <summary>
    /// Action method for form submission.
    /// Validates ownership logic and persists the new workout via the API client.
    /// </summary>
    /// <returns>A redirect to the index page or the current page if an error occurs.</returns>
    public async Task<IActionResult> OnPostAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
            return RedirectToPage("/Auth/Login");

        SetAdminStatus();
        AllExercises = await _exerciseApi.GetAllExercises(userId.Value);

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
