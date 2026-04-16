using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RepVector.UI.ViewModels;

namespace RepVector.UI.Pages.Workouts;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public List<WorkoutViewModel> Workouts { get; private set; } = new();

    public IndexModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task OnGetAsync()
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        var result = await client.GetFromJsonAsync<List<WorkoutViewModel>>("api/workouts");

        if (result != null)
        {
            Workouts = result;
        }
    }
}