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

        var sessionResult = await _sessionApi.GetActiveSession(userId.Value);
        if (sessionResult.IsFailure) return RedirectToPage("./Index");

        Session = sessionResult.Value!;

        // Load logs into the session object
        var logsResult = await _sessionApi.GetSessionLogs(Session.Id, userId.Value);
        Session.SetLogs = logsResult.Value ?? new();

        var workoutResult = await _workoutApi.GetWorkoutDetails(Session.WorkoutId.GetValueOrDefault(), userId.Value);
        Workout = workoutResult.Value;

        var prefsResult = await _prefApi.GetPreferences(userId.Value);
        Preferences = prefsResult.Value ?? new UserPreferences { UserId = userId.Value };

        var exercisesResult = await _exerciseApi.GetAllExercises(userId.Value);
        AllExercises = exercisesResult.Value ?? new();

        return Page();
    }

    /// AJAX action method to log a single set during the session.
    /// A JSON result indicating success or failure.
    public async Task<IActionResult> OnPostLogSetAsync(int exerciseId, decimal weight, int reps, int? rpe, string setType, int setNumber)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return new JsonResult(new { success = false });

        var sessionResult = await _sessionApi.GetActiveSession(userId.Value);
        if (sessionResult.IsFailure) return new JsonResult(new { success = false });

        var session = sessionResult.Value!;

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

        var logResult = await _sessionApi.LogSet(session.Id, log, userId.Value);
        return new JsonResult(new { success = logResult.IsSuccess, logId = logResult.Value?.Id });
    }

    /// AJAX action method to delete a logged set.
    public async Task<IActionResult> OnPostDeleteSetAsync(int logId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return new JsonResult(new { success = false });

        var sessionResult = await _sessionApi.GetActiveSession(userId.Value);
        if (sessionResult.IsFailure) return new JsonResult(new { success = false });

        var session = sessionResult.Value!;

        var result = await _sessionApi.DeleteSet(session.Id, logId, userId.Value);
        return new JsonResult(new { success = result.IsSuccess });
    }

    /// AJAX action method to delete all logs for an exercise.
    public async Task<IActionResult> OnPostDeleteExerciseLogsAsync(int exerciseId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return new JsonResult(new { success = false });

        var sessionResult = await _sessionApi.GetActiveSession(userId.Value);
        if (sessionResult.IsFailure) return new JsonResult(new { success = false });

        var session = sessionResult.Value!;

        var result = await _sessionApi.DeleteExerciseLogs(session.Id, exerciseId, userId.Value);
        return new JsonResult(new { success = result.IsSuccess });
    }

    /// Action method to finish the current session and optionally update the workout template.
    public async Task<IActionResult> OnPostFinishAsync(int totalSeconds, bool updateTemplate, List<WorkoutExercise> updatedExercises)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Auth/Login");

        var sessionResult = await _sessionApi.GetActiveSession(userId.Value);
        if (sessionResult.IsSuccess)
        {
            var session = sessionResult.Value!;
            session.TotalSeconds = totalSeconds;
            session.Status = "completed";
            await _sessionApi.UpdateStatus(session.Id, "completed", userId.Value);

            if (updateTemplate)
            {
                var workoutResult = await _workoutApi.GetWorkoutDetails(session.WorkoutId.GetValueOrDefault(), userId.Value);
                if (workoutResult.IsSuccess)
                {
                    var workout = workoutResult.Value!;
                    if (workout.UserId == userId || HttpContext.Session.GetString("UserRole") == "Admin")
                    {
                        workout.Exercises = updatedExercises;
                        await _workoutApi.UpdateWorkout(workout, userId.Value);
                    }
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

        var sessionResult = await _sessionApi.GetActiveSession(userId.Value);
        if (sessionResult.IsSuccess)
        {
            await _sessionApi.DeleteSession(sessionResult.Value!.Id, userId.Value);
        }

        return RedirectToPage("./Index");
    }
}
