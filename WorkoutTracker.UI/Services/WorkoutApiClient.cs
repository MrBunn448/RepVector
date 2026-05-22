using System.Net.Http.Json;
using WorkoutTracker.Models;

namespace WorkoutTracker.UI.Services;

/// Handles retrieval and management of workout definitions, passing user identification in request headers.
public class WorkoutApiClient
{
    private readonly HttpClient _httpClient;

    public WorkoutApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// returns A list of workout templates.
    public async Task<List<Workout>> GetWorkouts(int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/workouts/user/{userId}");
        request.Headers.Add("X-User-Id", userId.ToString());
        
        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<Workout>>() ?? new List<Workout>();
        }
        return new List<Workout>();
    }

    /// Retrieves a specific workout template along with its exercise list.
    public async Task<Workout?> GetWorkoutDetails(int id, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/workouts/{id}/details");
        request.Headers.Add("X-User-Id", userId.ToString());

        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Workout>();
        }
        return null;
    }


    /// Creates a new workout template definition.
    /// <returns>The ID of the newly created workout template.</returns>
    public async Task<int> CreateWorkout(Workout workout, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/workouts");
        request.Headers.Add("X-User-Id", userId.ToString());
        request.Content = JsonContent.Create(workout);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var location = response.Headers.Location?.ToString();
        if (!string.IsNullOrEmpty(location) && location.Split('/').Last() is string id && int.TryParse(id, out var workoutId))
        {
            return workoutId;
        }
        return 0;
    }

    public async Task UpdateWorkout(Workout workout, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, $"api/workouts/{workout.Id}");
        request.Headers.Add("X-User-Id", userId.ToString());
        request.Content = JsonContent.Create(workout);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteWorkout(int id, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"api/workouts/{id}");
        request.Headers.Add("X-User-Id", userId.ToString());

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}