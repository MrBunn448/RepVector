using System.Net.Http.Json;
using WorkoutTracker.Models;

namespace WorkoutTracker.UI.Services;

public class WorkoutApiClient
{
    private readonly HttpClient _http;

    public WorkoutApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Workout>> GetWorkouts()
        => await _http.GetFromJsonAsync<List<Workout>>("api/workouts")
           ?? new List<Workout>();

    public async Task<Workout?> GetWorkout(int id)
        => await _http.GetFromJsonAsync<Workout>($"api/workouts/{id}");
}