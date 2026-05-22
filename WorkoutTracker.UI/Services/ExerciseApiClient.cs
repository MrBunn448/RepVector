using System.Net.Http.Json;
using WorkoutTracker.Models;

namespace WorkoutTracker.UI.Services;

/// Handles retrieval and management of exercises, passing user identification in request headers.
public class ExerciseApiClient
{
    private readonly HttpClient _httpClient;


    /// Initializes a new instance of the ExerciseApiClient.
    public ExerciseApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// Retrieves all exercises available from the specified user.
    public async Task<List<Exercise>> GetAllExercises(int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/exercises");
        request.Headers.Add("X-User-Id", userId.ToString());

        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<Exercise>>() ?? new List<Exercise>();
        }
        return new List<Exercise>();
    }

    /// returns The Exercise object if successful, otherwise null
    public async Task<Exercise?> GetExerciseById(int id, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/exercises/{id}");
        request.Headers.Add("X-User-Id", userId.ToString());

        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Exercise>();
        }
        return null;
    }

    /// returns The ID of the newly created exercise
    public async Task<int> CreateExercise(Exercise exercise, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/exercises");
        request.Headers.Add("X-User-Id", userId.ToString());
        request.Content = JsonContent.Create(exercise);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var location = response.Headers.Location?.ToString();
        if (!string.IsNullOrEmpty(location) && location.Split('/').Last() is string id && int.TryParse(id, out var exerciseId))
        {
            return exerciseId;
        }
        return 0;
    }

    public async Task UpdateExercise(Exercise exercise, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, $"api/exercises/{exercise.Id}");
        request.Headers.Add("X-User-Id", userId.ToString());
        request.Content = JsonContent.Create(exercise);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteExercise(int id, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"api/exercises/{id}");
        request.Headers.Add("X-User-Id", userId.ToString());

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}
