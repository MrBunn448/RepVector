using System.Net.Http.Json;
using WorkoutTracker.Models;

namespace WorkoutTracker.UI.Services;

/// Manages the links between workouts and specific movements.
/// Inherits from BaseApiClient for standardized error handling.
public class WorkoutExerciseApiClient(HttpClient httpClient) : BaseApiClient(httpClient)
{
    /// Retrieves all exercises linked to a specific workout template.
    public async Task<Result<List<WorkoutExercise>>> GetByWorkoutId(int workoutId, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/workout-exercises/{workoutId}");
        request.Headers.Add("X-User-Id", userId.ToString());

        var response = await HttpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<List<WorkoutExercise>>() ?? new List<WorkoutExercise>();
            return Result<List<WorkoutExercise>>.Success(data);
        }
        
        return await HandleFailure<List<WorkoutExercise>>(response);
    }

    /// Adds a new exercise link to a workout template.
    public async Task<Result> Add(object data, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/workout-exercises");
        request.Headers.Add("X-User-Id", userId.ToString());
        request.Content = JsonContent.Create(data);

        var response = await HttpClient.SendAsync(request);
        if (response.IsSuccessStatusCode) return Result.Success();

        return await HandleFailure(response);
    }

    /// Updates an existing exercise link in a workout template.
    public async Task<Result> Update(int id, object data, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, $"api/workout-exercises/{id}");
        request.Headers.Add("X-User-Id", userId.ToString());
        request.Content = JsonContent.Create(data);

        var response = await HttpClient.SendAsync(request);
        if (response.IsSuccessStatusCode) return Result.Success();

        return await HandleFailure(response);
    }

    /// Removes an exercise link from a workout template.
    public async Task<Result> Delete(int id, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"api/workout-exercises/{id}");
        request.Headers.Add("X-User-Id", userId.ToString());

        var response = await HttpClient.SendAsync(request);
        if (response.IsSuccessStatusCode) return Result.Success();

        return await HandleFailure(response);
    }
}
