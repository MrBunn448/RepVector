using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RepVector.UI.ViewModels;

namespace RepVector.UI.Pages.Workouts;

public class DetailsModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public WorkoutDetailViewModel? Workout { get; private set; }

    public DetailsModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        Workout = await client.GetFromJsonAsync<WorkoutDetailViewModel>($"api/workouts/{id}");

        if (Workout == null)
        {
            return NotFound();
        }

        return Page();
    }
}