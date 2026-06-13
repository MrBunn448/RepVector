using System.Net.Http.Json;
using WorkoutTracker.Models;

namespace WorkoutTracker.UI.Services;

/// Handles retrieval and management of workout definitions, passing user identification in request headers.
/// Inherits from BaseApiClient for standardized error handling.
public class WorkoutApiClient(HttpClient httpClient) : BaseApiClient(httpClient)
{
    /// Retrieves all workout templates accessible to the specified user.
    public async Task<Result<List<Workout>>> GetWorkouts(int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/workouts/user/{userId}");
        request.Headers.Add("X-User-Id", userId.ToString());
        
        var response = await HttpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<List<Workout>>() ?? new List<Workout>();
            return Result<List<Workout>>.Success(data);
        }
        
        return await HandleFailure<List<Workout>>(response);
    }

    /// Retrieves a specific workout template along with its exercise list.
    public async Task<Result<Workout>> GetWorkoutDetails(int id, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/workouts/{id}/details");
        request.Headers.Add("X-User-Id", userId.ToString());

        var response = await HttpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<Workout>();
            return data != null ? Result<Workout>.Success(data) : Result<Workout>.NotFound();
        }
        
        return await HandleFailure<Workout>(response);
    }

    /// Creates a new workout template definition.
    public async Task<Result<int>> CreateWorkout(Workout workout, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/workouts");
        request.Headers.Add("X-User-Id", userId.ToString());
        request.Content = JsonContent.Create(workout);

        var response = await HttpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var location = response.Headers.Location?.ToString();
            if (!string.IsNullOrEmpty(location) && location.Split('/').Last() is string id && int.TryParse(id, out var workoutId))
            {
                return Result<int>.Success(workoutId);
            }
            return Result<int>.Failure("Workout created but ID was not returned.");
        }

        return await HandleFailure<int>(response);
    }

    /// Updates an existing workout template definition.
    public async Task<Result> UpdateWorkout(Workout workout, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, $"api/workouts/{workout.Id}");
        request.Headers.Add("X-User-Id", userId.ToString());
        request.Content = JsonContent.Create(workout);

        var response = await HttpClient.SendAsync(request);
        if (response.IsSuccessStatusCode) return Result.Success();

        return await HandleFailure(response);
    }

    /// Removes a workout template definition.
    public async Task<Result> DeleteWorkout(int id, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"api/workouts/{id}");
        request.Headers.Add("X-User-Id", userId.ToString());

        var response = await HttpClient.SendAsync(request);
        if (response.IsSuccessStatusCode) return Result.Success();

        return await HandleFailure(response);
    }
}
