using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WorkoutTracker.Models;
using WorkoutTracker.UI.Services;

namespace WorkoutTracker.UI.Pages.Workouts;

/// <summary>
/// Page model for viewing workout template details.
/// Handles exercise management within a template (add/remove) and starting new sessions.
/// </summary>
public class DetailsModel : PageModel
{
    private readonly WorkoutApiClient _apiClient;
    private readonly WorkoutExerciseApiClient _workoutExercises;
    private readonly ExerciseApiClient _exerciseApi;
    private readonly SessionApiClient _sessionApi;

    /// <summary>
    /// Initializes a new instance of the DetailsModel.
    /// </summary>
    public DetailsModel(
        WorkoutApiClient apiClient,
        WorkoutExerciseApiClient workoutExercises,
        ExerciseApiClient exerciseApi,
        SessionApiClient sessionApi)
    {
        _apiClient = apiClient;
        _workoutExercises = workoutExercises;
        _exerciseApi = exerciseApi;
        _sessionApi = sessionApi;
    }

    /// <summary> Gets the workout metadata. </summary>
    public Workout? Workout { get; private set; }
    /// <summary> Gets the list of exercises currently linked to this workout. </summary>
    public List<WorkoutExercise> Exercises { get; private set; } = new();
    /// <summary> Gets the list of global exercises available to add. </summary>
    public List<Exercise> RepVectorExercises { get; private set; } = new();
    /// <summary> Gets the list of personal exercises available to add. </summary>
    public List<Exercise> PersonalExercises { get; private set; } = new();

    /// <summary> Gets or sets the ID of the exercise selected to be added to the workout. </summary>
    [BindProperty]
    public int SelectedExerciseId { get; set; }

    /// <summary> Gets or sets the target sets for the new exercise link. </summary>
    [BindProperty]
    public int TargetSets { get; set; } = 3;

    /// <summary> Gets or sets the target repetitions for the new exercise link. </summary>
    [BindProperty]
    public int TargetReps { get; set; } = 10;

    /// <summary> Gets or sets an error message for session start conflicts. </summary>
    public string? ConflictError { get; set; }

    /// <summary>
    /// Lifecycle method executed on GET requests.
    /// Loads the workout, its linked exercises, and the catalog of available movements.
    /// </summary>
    /// <param name="id">The ID of the workout to view.</param>
    /// <returns>A Page result, a redirect if unauthenticated, or a 404/403 if unauthorized.</returns>
    public async Task<IActionResult> OnGet(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
            return RedirectToPage("/Auth/Login");

        Workout = await _apiClient.GetWorkoutDetails(id, userId.Value);
        if (Workout == null) return NotFound();

        if (Workout.UserId != userId.Value && !Workout.IsPredefined)
            return Forbid();

        Exercises = (Workout.Exercises != null && Workout.Exercises.Any()) 
            ? Workout.Exercises 
            : await _workoutExercises.GetByWorkoutId(id, userId.Value);

        var allExercises = await _exerciseApi.GetAllExercises(userId.Value);
        RepVectorExercises = allExercises.Where(e => e.IsPredefined).ToList();
        PersonalExercises = allExercises.Where(e => !e.IsPredefined).ToList();

        return Page();
    }

    /// <summary>
    /// Action method to start a new workout session based on this template.
    /// </summary>
    /// <param name="id">The ID of the workout template.</param>
    /// <param name="force">If true, cancels any existing active session.</param>
    /// <returns>A redirect to the active session page or current page on conflict.</returns>
    public async Task<IActionResult> OnPostStartSessionAsync(int id, bool force = false)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Auth/Login");

        var (success, error) = await _sessionApi.StartSession(id, userId.Value, force);
        if (success)
        {
            return RedirectToPage("./Active");
        }

        if (error == "AN_ACTIVE_SESSION_EXISTS")
        {
            ConflictError = "You already have an ongoing workout session.";
            return await OnGet(id);
        }

        return await OnGet(id);
    }

    /// <summary>
    /// Action method to add a movement link to the workout template.
    /// </summary>
    /// <param name="id">The ID of the workout template.</param>
    /// <returns>A redirect back to the details page.</returns>
    public async Task<IActionResult> OnPostAddExerciseAsync(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Auth/Login");

        var workout = await _apiClient.GetWorkoutDetails(id, userId.Value);
        if (workout == null) return NotFound();

        bool isAdmin = HttpContext.Session.GetString("UserRole") == "Admin";
        if (workout.IsPredefined && !isAdmin)
            return Forbid();

        await _workoutExercises.Add(new
        {
            WorkoutId = id,
            ExerciseId = SelectedExerciseId,
            TargetSets = TargetSets,
            TargetReps = TargetReps,
            SortOrder = 0 
        }, userId.Value);

        return RedirectToPage(new { id });
    }

    /// <summary>
    /// Action method to remove a movement link from the workout template.
    /// </summary>
    /// <param name="id">The ID of the workout template.</param>
    /// <param name="exerciseId">The ID of the link to remove.</param>
    /// <returns>A redirect back to the details page.</returns>
    public async Task<IActionResult> OnPostDeleteExerciseAsync(int id, int exerciseId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Auth/Login");

        var workout = await _apiClient.GetWorkoutDetails(id, userId.Value);
        if (workout == null) return NotFound();

        bool isAdmin = HttpContext.Session.GetString("UserRole") == "Admin";
        if (workout.IsPredefined && !isAdmin)
            return Forbid();

        await _workoutExercises.Delete(exerciseId, userId.Value);
        return RedirectToPage(new { id });
    }
}
