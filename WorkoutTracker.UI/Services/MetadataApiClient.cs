using System.Net.Http.Json;
using WorkoutTracker.Models;

namespace WorkoutTracker.UI.Services;

public class MetadataApiClient
{
    private readonly HttpClient _httpClient;

    public MetadataApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// Retrieves all muscle groups from the API.
    public async Task<List<MuscleGroup>> GetMuscleGroups()
    {
        return await _httpClient.GetFromJsonAsync<List<MuscleGroup>>("api/muscle-groups") ?? new();
    }

    /// Creates a new muscle group. Requires admin privileges via the userId identification.
    public async Task CreateMuscleGroup(MuscleGroup mg, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/muscle-groups");
        request.Headers.Add("X-User-Id", userId.ToString());
        request.Content = JsonContent.Create(mg);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}
