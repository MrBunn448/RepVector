using System.Net.Http.Json;
using WorkoutTracker.Models;

namespace WorkoutTracker.UI.Services;

/// Manages the links between workouts and specific movements, passing user identification in request headers.
public class WorkoutExerciseApiClient
{
    private readonly HttpClient _http;

    public WorkoutExerciseApiClient(HttpClient http)
    {
        _http = http;
    }

    /// Retrieves all exercises linked to a specific workout template.
    public async Task<List<WorkoutExercise>> GetByWorkoutId(int workoutId, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/workout-exercises/{workoutId}");
        request.Headers.Add("X-User-Id", userId.ToString());

        var response = await _http.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<WorkoutExercise>>() ?? new List<WorkoutExercise>();
        }
        return new List<WorkoutExercise>();
    }

    /// Adds a new exercise link to a workout template.
    public async Task Add(object data, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/workout-exercises");
        request.Headers.Add("X-User-Id", userId.ToString());
        request.Content = JsonContent.Create(data);

        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    /// Updates an existing exercise link in a workout template.
    public async Task Update(int id, object data, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, $"api/workout-exercises/{id}");
        request.Headers.Add("X-User-Id", userId.ToString());
        request.Content = JsonContent.Create(data);

        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    /// Removes an exercise link from a workout template.
    public async Task Delete(int id, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"api/workout-exercises/{id}");
        request.Headers.Add("X-User-Id", userId.ToString());

        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}