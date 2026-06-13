using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WorkoutTracker.Models;
using WorkoutTracker.UI.Services;

namespace WorkoutTracker.UI.Pages.Workouts;

public class DetailsModel : PageModel
{
    private readonly WorkoutApiClient _apiClient;
    private readonly WorkoutExerciseApiClient _workoutExercises;
    private readonly ExerciseApiClient _exerciseApi;
    private readonly SessionApiClient _sessionApi;

    /// Initializes a new instance of the DetailsModel.
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

    public Workout? Workout { get; private set; }
    public List<WorkoutExercise> Exercises { get; private set; } = new();
    public List<Exercise> RepVectorExercises { get; private set; } = new();
    public List<Exercise> PersonalExercises { get; private set; } = new();

    [BindProperty]
    public int SelectedExerciseId { get; set; }

    [BindProperty]
    public int TargetSets { get; set; } = 3;

    [BindProperty]
    public int TargetReps { get; set; } = 10;

    public string? ConflictError { get; set; }

    public Result<Workout>? WorkoutResult { get; set; }
    public Result<List<WorkoutExercise>>? WorkoutExercisesResult { get; set; }

    /// Loads the workout, its linked exercises, and the catalog of available movements.
    public async Task<IActionResult> OnGet(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
            return RedirectToPage("/Auth/Login");

        WorkoutResult = await _apiClient.GetWorkoutDetails(id, userId.Value);
        if (WorkoutResult.IsFailure)
        {
            return WorkoutResult.Type == ResultType.NotFound ? NotFound() : Page();
        }

        Workout = WorkoutResult.Value!;

        if (Workout.UserId != userId.Value && !Workout.IsPredefined)
            return Forbid();

        if (Workout.Exercises != null && Workout.Exercises.Any())
        {
            Exercises = Workout.Exercises;
        }
        else
        {
            WorkoutExercisesResult = await _workoutExercises.GetByWorkoutId(id, userId.Value);
            Exercises = WorkoutExercisesResult.Value ?? new();
        }

        var result = await _exerciseApi.GetAllExercises(userId.Value);
        var allExercises = result.Value ?? new List<Exercise>();
        RepVectorExercises = allExercises.Where(e => e.IsPredefined).ToList();
        PersonalExercises = allExercises.Where(e => !e.IsPredefined).ToList();

        return Page();
    }

    /// Action method to start a new workout session based on this template.
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


    /// Action method to add a movement link to the workout template.
    public async Task<IActionResult> OnPostAddExerciseAsync(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Auth/Login");

        var workoutResult = await _apiClient.GetWorkoutDetails(id, userId.Value);
        if (workoutResult.IsFailure) return NotFound();
        var workout = workoutResult.Value!;

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

    /// Action method to remove a movement link from the workout template.
    public async Task<IActionResult> OnPostDeleteExerciseAsync(int id, int exerciseId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Auth/Login");

        var workoutResult = await _apiClient.GetWorkoutDetails(id, userId.Value);
        if (workoutResult.IsFailure) return NotFound();
        var workout = workoutResult.Value!;

        bool isAdmin = HttpContext.Session.GetString("UserRole") == "Admin";
        if (workout.IsPredefined && !isAdmin)
            return Forbid();

        await _workoutExercises.Delete(exerciseId, userId.Value);
        return RedirectToPage(new { id });
    }
}
