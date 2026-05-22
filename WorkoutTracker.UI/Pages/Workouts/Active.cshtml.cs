using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WorkoutTracker.Models;
using WorkoutTracker.UI.Services;

namespace WorkoutTracker.UI.Pages.Workouts;

/// <summary>
/// Page model for the active workout session.
/// Handles real-time set logging, session completion, and template synchronization.
/// </summary>
public class ActiveModel : PageModel
{
    private readonly SessionApiClient _sessionApi;
    private readonly WorkoutApiClient _workoutApi;
    private readonly PreferenceApiClient _prefApi;
    private readonly ExerciseApiClient _exerciseApi;

    /// <summary> Gets or sets the current active workout session. </summary>
    public WorkoutSession? Session { get; set; }
    /// <summary> Gets or sets the workout template being performed. </summary>
    public Workout? Workout { get; set; }
    /// <summary> Gets or sets user preferences for unit display. </summary>
    public UserPreferences Preferences { get; set; } = new();
    /// <summary> Gets or sets the list of all available exercises for selection. </summary>
    public List<Exercise> AllExercises { get; set; } = new();
    /// <summary> Gets or sets the list of all available workouts for switching. </summary>
    public List<Workout> AllWorkouts { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of the ActiveModel.
    /// </summary>
    public ActiveModel(SessionApiClient sessionApi, WorkoutApiClient workoutApi, PreferenceApiClient prefApi, ExerciseApiClient exerciseApi)
    {
        _sessionApi = sessionApi;
        _workoutApi = workoutApi;
        _prefApi = prefApi;
        _exerciseApi = exerciseApi;
    }

    /// <summary>
    /// Lifecycle method executed on GET requests.
    /// Fetches the active session and related data to populate the UI.
    /// </summary>
    /// <returns>A Page result or a redirect if no session is active.</returns>
    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Auth/Login");

        Session = await _sessionApi.GetActiveSession(userId.Value);
        if (Session == null) return RedirectToPage("./Index");

        Workout = await _workoutApi.GetWorkoutDetails(Session.WorkoutId.GetValueOrDefault(), userId.Value);
        Preferences = await _prefApi.GetPreferences(userId.Value);
        AllExercises = await _exerciseApi.GetAllExercises(userId.Value);
        AllWorkouts = await _workoutApi.GetWorkouts(userId.Value);

        return Page();
    }

    /// <summary>
    /// AJAX action method to log a single set during the session.
    /// </summary>
    /// <param name="exerciseId">The ID of the exercise performed.</param>
    /// <param name="weight">The weight lifted.</param>
    /// <param name="reps">The number of repetitions.</param>
    /// <param name="rpe">The Rate of Perceived Exertion.</param>
    /// <param name="setType">The type of set (e.g., Normal, Warmup).</param>
    /// <param name="setNumber">The sequence number of the set.</param>
    /// <returns>A JSON result indicating success or failure.</returns>
    public async Task<IActionResult> OnPostLogSetAsync(int exerciseId, decimal weight, int reps, int? rpe, string setType, int setNumber)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return new JsonResult(new { success = false });

        var session = await _sessionApi.GetActiveSession(userId.Value);
        if (session == null) return new JsonResult(new { success = false });

        var log = new WorkoutSetLog
        {
            SessionId = session.Id,
            ExerciseId = exerciseId,
            Weight = weight,
            Reps = reps,
            Rpe = rpe,
            SetType = setType,
            SetNumber = setNumber
        };

        await _sessionApi.LogSet(session.Id, log, userId.Value);
        return new JsonResult(new { success = true });
    }

    /// <summary>
    /// Action method to finish the current session and optionally update the workout template.
    /// </summary>
    /// <param name="totalSeconds">The total duration of the session in seconds.</param>
    /// <param name="updateTemplate">Whether to save the current exercise targets back to the template.</param>
    /// <param name="updatedExercises">The list of exercises with updated targets.</param>
    /// <returns>A redirect to the user profile page.</returns>
    public async Task<IActionResult> OnPostFinishAsync(int totalSeconds, bool updateTemplate, List<WorkoutExercise> updatedExercises)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Auth/Login");

        var session = await _sessionApi.GetActiveSession(userId.Value);
        if (session != null)
        {
            session.TotalSeconds = totalSeconds;
            session.Status = "completed";
            await _sessionApi.UpdateStatus(session.Id, "completed", userId.Value);

            if (updateTemplate)
            {
                var workout = await _workoutApi.GetWorkoutDetails(session.WorkoutId.GetValueOrDefault(), userId.Value);
                if (workout != null && (workout.UserId == userId || HttpContext.Session.GetString("UserRole") == "Admin"))
                {
                    workout.Exercises = updatedExercises;
                    await _workoutApi.UpdateWorkout(workout, userId.Value);
                }
            }
        }

        return RedirectToPage("/User/Index");
    }

    /// <summary>
    /// Action method to cancel the current session and delete its logs.
    /// </summary>
    /// <returns>A redirect to the workout list index.</returns>
    public async Task<IActionResult> OnPostCancelAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Auth/Login");

        var session = await _sessionApi.GetActiveSession(userId.Value);
        if (session != null)
        {
            await _sessionApi.DeleteSession(session.Id, userId.Value);
        }

        return RedirectToPage("./Index");
    }

    /// <summary>
    /// AJAX action method to fetch detailed exercise list for a workout template.
    /// Used when switching workouts during an active session.
    /// </summary>
    /// <param name="id">The ID of the workout template.</param>
    /// <returns>A JSON result with simplified workout exercise data.</returns>
    public async Task<IActionResult> OnGetWorkoutDetailsAsync(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return new JsonResult(null);

        var workout = await _workoutApi.GetWorkoutDetails(id, userId.Value);
        if (workout == null) return new JsonResult(null);

        // Map to a simpler structure for JS
        var result = workout.Exercises.Select(we => new
        {
            we.ExerciseId,
            ExerciseName = we.Exercise?.Name ?? "Unknown",
            we.TargetSets,
            we.TargetReps
        });

        return new JsonResult(result);
    }
}
