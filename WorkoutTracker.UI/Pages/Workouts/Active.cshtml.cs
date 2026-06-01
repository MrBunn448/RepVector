using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WorkoutTracker.Models;
using WorkoutTracker.UI.Services;

namespace WorkoutTracker.UI.Pages.Workouts;

public class ActiveModel : PageModel
{
    private readonly SessionApiClient _sessionApi;
    private readonly WorkoutApiClient _workoutApi;
    private readonly PreferenceApiClient _prefApi;
    private readonly ExerciseApiClient _exerciseApi;

    public WorkoutSession? Session { get; set; }
    public Workout? Workout { get; set; }
    public UserPreferences Preferences { get; set; } = new();
    public List<Exercise> AllExercises { get; set; } = new();

    public ActiveModel(SessionApiClient sessionApi, WorkoutApiClient workoutApi, PreferenceApiClient prefApi, ExerciseApiClient exerciseApi)
    {
        _sessionApi = sessionApi;
        _workoutApi = workoutApi;
        _prefApi = prefApi;
        _exerciseApi = exerciseApi;
    }

    /// Fetches the active session and related data to populate the UI.
    /// A Page result or a redirect if no session is active.
    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Auth/Login");

        Session = await _sessionApi.GetActiveSession(userId.Value);
        if (Session == null) return RedirectToPage("./Index");

        Workout = await _workoutApi.GetWorkoutDetails(Session.WorkoutId.GetValueOrDefault(), userId.Value);
        Preferences = await _prefApi.GetPreferences(userId.Value);
        AllExercises = await _exerciseApi.GetAllExercises(userId.Value);

        return Page();
    }

    /// AJAX action method to log a single set during the session.
    /// A JSON result indicating success or failure.
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

    /// Action method to finish the current session and optionally update the workout template.
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

    /// Action method to cancel the current session and delete its logs.
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
}
