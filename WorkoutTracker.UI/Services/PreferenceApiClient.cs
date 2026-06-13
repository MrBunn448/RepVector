using System.Net.Http.Json;
using WorkoutTracker.Models;

namespace WorkoutTracker.UI.Services;

/// Manages user preferences and measurement units.
/// Inherits from BaseApiClient for standardized error handling.
public class PreferenceApiClient(HttpClient httpClient) : BaseApiClient(httpClient)
{
    /// Retrieves the preferences for a specific user.
    public async Task<Result<UserPreferences>> GetPreferences(int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/user-preferences");
        request.Headers.Add("X-User-Id", userId.ToString());

        var response = await HttpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<UserPreferences>();
            return data != null ? Result<UserPreferences>.Success(data) : Result<UserPreferences>.NotFound();
        }

        return await HandleFailure<UserPreferences>(response);
    }

    /// Saves or updates the preferences for a user.
    public async Task<Result> SavePreferences(UserPreferences preferences, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/user-preferences");
        request.Headers.Add("X-User-Id", userId.ToString());
        request.Content = JsonContent.Create(preferences);

        var response = await HttpClient.SendAsync(request);
        if (response.IsSuccessStatusCode) return Result.Success();

        return await HandleFailure(response);
    }
}
