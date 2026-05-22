using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WorkoutTracker.Models;
using WorkoutTracker.UI.Services;

namespace WorkoutTracker.UI.Pages.Exercises;

/// <summary>
/// Page model for editing an existing exercise.
/// Handles permission checks and updates metadata categories.
/// </summary>
public class EditModel : PageModel
{
    private readonly ExerciseApiClient _api;
    private readonly MetadataApiClient _metadataApi;

    /// <summary> Gets or sets the exercise being edited. </summary>
    [BindProperty]
    public Exercise Exercise { get; set; } = new();

    /// <summary> Gets or sets the list of muscle groups for selection. </summary>
    public List<SelectListItem> MuscleGroups { get; set; } = new();
    /// <summary> Gets or sets the list of exercise types for selection. </summary>
    public List<SelectListItem> ExerciseTypes { get; set; } = new();

    /// <summary> Gets or sets a value indicating whether the current user is an administrator. </summary>
    public bool IsAdmin { get; set; }

    /// <summary>
    /// Initializes a new instance of the EditModel.
    /// </summary>
    public EditModel(ExerciseApiClient api, MetadataApiClient metadataApi)
    {
        _api = api;
        _metadataApi = metadataApi;
    }

    /// <summary>
    /// Lifecycle method executed on GET requests.
    /// Fetches the existing exercise and validates that the user has permission to edit it.
    /// </summary>
    /// <param name="id">The ID of the exercise to edit.</param>
    /// <returns>A Page result, a 404 if not found, or a 403 if unauthorized.</returns>
    public async Task<IActionResult> OnGet(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
            return RedirectToPage("/Auth/Login");

        IsAdmin = HttpContext.Session.GetString("UserRole") == "Admin";
        Exercise = await _api.GetExerciseById(id, userId.Value);

        if (Exercise == null)
            return NotFound();

        // Permission check
        if (Exercise.UserId != userId && !IsAdmin)
            return Forbid();

        await LoadMetadata();
        return Page();
    }

    /// <summary>
    /// Internal helper to load dropdown lists for the view.
    /// </summary>
    private async Task LoadMetadata()
    {
        var groups = await _metadataApi.GetMuscleGroups();
        MuscleGroups = groups.Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Name }).ToList();

        var types = new[] { "Machine", "Bodyweight", "Barbell", "Dumbell", "Kettlebell", "Plate", "Resistance Band", "Suspension Band", "Other" };
        ExerciseTypes = types.Select(t => new SelectListItem { Value = t, Text = t }).ToList();
    }

    /// <summary>
    /// Action method for form submission.
    /// Performs a final permission check and updates the exercise record.
    /// </summary>
    /// <returns>A redirect to the index page or the current page if validation fails.</returns>
    public async Task<IActionResult> OnPost()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
            return RedirectToPage("/Auth/Login");

        IsAdmin = HttpContext.Session.GetString("UserRole") == "Admin";

        if (!ModelState.IsValid)
        {
            await LoadMetadata();
            return Page();
        }

        // Permission check
        var existing = await _api.GetExerciseById(Exercise.Id, userId.Value);
        if (existing == null) return NotFound();
        if (existing.UserId != userId && !IsAdmin) return Forbid();

        if (!IsAdmin) Exercise.IsPredefined = false;

        await _api.UpdateExercise(Exercise, userId.Value);
        return RedirectToPage("./Index");
    }
}
