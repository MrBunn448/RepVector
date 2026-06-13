using System.Net.Http.Json;
using WorkoutTracker.Models;

namespace WorkoutTracker.UI.Services;

/// Manages global system metadata such as muscle groups.
/// Inherits from BaseApiClient for standardized error handling.
public class MetadataApiClient(HttpClient httpClient) : BaseApiClient(httpClient)
{
    /// Retrieves all muscle groups from the API.
    public async Task<Result<List<MuscleGroup>>> GetMuscleGroups()
    {
        var response = await HttpClient.GetAsync("api/muscle-groups");
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<List<MuscleGroup>>() ?? new();
            return Result<List<MuscleGroup>>.Success(data);
        }

        return await HandleFailure<List<MuscleGroup>>(response);
    }

    /// Creates a new muscle group. Requires admin privileges.
    public async Task<Result> CreateMuscleGroup(MuscleGroup mg, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/muscle-groups");
        request.Headers.Add("X-User-Id", userId.ToString());
        request.Content = JsonContent.Create(mg);

        var response = await HttpClient.SendAsync(request);
        if (response.IsSuccessStatusCode) return Result.Success();

        return await HandleFailure(response);
    }
}
