using System.Net.Http.Json;
using WorkoutTracker.Models;

namespace WorkoutTracker.UI.Services;

public class WorkoutApiClient
{
    private readonly HttpClient _httpClient;

    public WorkoutApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Workout>> GetWorkouts()
    {
        return await _httpClient.GetFromJsonAsync<List<Workout>>("api/workouts")
               ?? new List<Workout>();
    }

    public async Task<Workout?> GetWorkoutDetails(int id)
    {
        return await _httpClient.GetFromJsonAsync<Workout>($"api/workouts/{id}");
    }
}