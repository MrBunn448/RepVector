using System.Net.Http.Json;
using WorkoutTracker.Models;

namespace WorkoutTracker.UI.Services;

public class PreferenceApiClient
{
    private readonly HttpClient _httpClient;

    public PreferenceApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// Retrieves preferences for the specified user.
    public async Task<UserPreferences> GetPreferences(int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/user-preferences");
        request.Headers.Add("X-User-Id", userId.ToString());

        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<UserPreferences>() ?? new UserPreferences { UserId = userId };
        
        return new UserPreferences { UserId = userId };
    }

    /// Saves or updates preferences for the specified user.
    public async Task SavePreferences(UserPreferences prefs, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/user-preferences");
        request.Headers.Add("X-User-Id", userId.ToString());
        request.Content = JsonContent.Create(prefs);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}
