using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WorkoutTracker.Models;
using WorkoutTracker.UI.Services;

namespace WorkoutTracker.UI.Pages.Exercises;
/// Handles metadata loading for dropdowns and ensures role-based constraints on predefined exercises.
public class CreateModel : PageModel
{
    private readonly ExerciseApiClient _api;
    private readonly MetadataApiClient _metadataApi;
    [BindProperty]
    public Exercise Exercise { get; set; } = new();

    public List<SelectListItem> MuscleGroups { get; set; } = new();
    public List<SelectListItem> ExerciseTypes { get; set; } = new();

    public bool IsAdmin { get; set; }

    /// Initializes a new instance of the CreateModel.
    public CreateModel(ExerciseApiClient api, MetadataApiClient metadataApi)
    {
        _api = api;
        _metadataApi = metadataApi;
    }


    /// Exercutes On Page load
    /// Prepares the form by loading necessary metadata and checking user roles.
    /// returns A Page result or a redirect to login if unauthenticated.
    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Auth/Login");

        IsAdmin = HttpContext.Session.GetString("UserRole") == "Admin";
        if (IsAdmin) Exercise.IsPredefined = true;

        await LoadMetadata();
        return Page();
    }


    /// Internal helper to fetch and format metadata for UI selection lists
    private async Task LoadMetadata()
    {
        var groups = await _metadataApi.GetMuscleGroups();
        MuscleGroups = groups.Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Name }).ToList();

        var types = new[] { "Machine", "Bodyweight", "Barbell", "Dumbell", "Kettlebell", "Plate", "Resistance Band", "Suspension Band", "Other" };
        ExerciseTypes = types.Select(t => new SelectListItem { Value = t, Text = t }).ToList();
    }


    /// form submission.
    /// Validates input and creates the new exercise trough the API
    /// Redirect to the index page or the current page if validation fails
    public async Task<IActionResult> OnPostAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Auth/Login");

        IsAdmin = HttpContext.Session.GetString("UserRole") == "Admin";

        if (!ModelState.IsValid)
        {
            await LoadMetadata();
            return Page();
        }

        if (!IsAdmin) Exercise.IsPredefined = false;
        
        Exercise.CreatedAt = DateTime.UtcNow;

        await _api.CreateExercise(Exercise, userId.Value);
        return RedirectToPage("./Index");
    }
}
