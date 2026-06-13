using System.Net.Http.Json;
using WorkoutTracker.Models;

namespace WorkoutTracker.UI.Services;

/// Handles retrieval and management of exercises, passing user identification in request headers.
/// Inherits from BaseApiClient for standardized error handling.
public class ExerciseApiClient(HttpClient httpClient) : BaseApiClient(httpClient)
{
    /// Retrieves all exercises available to the specified user.
    public async Task<Result<List<Exercise>>> GetAllExercises(int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/exercises");
        request.Headers.Add("X-User-Id", userId.ToString());

        var response = await HttpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<List<Exercise>>() ?? new List<Exercise>();
            return Result<List<Exercise>>.Success(data);
        }

        return await HandleFailure<List<Exercise>>(response);
    }

    /// Retrieves a specific exercise by its identifier.
    public async Task<Result<Exercise>> GetExerciseById(int id, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/exercises/{id}");
        request.Headers.Add("X-User-Id", userId.ToString());

        var response = await HttpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<Exercise>();
            return data != null ? Result<Exercise>.Success(data) : Result<Exercise>.NotFound();
        }

        return await HandleFailure<Exercise>(response);
    }

    /// Creates a new exercise definition.
    public async Task<Result<int>> CreateExercise(Exercise exercise, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/exercises");
        request.Headers.Add("X-User-Id", userId.ToString());
        request.Content = JsonContent.Create(exercise);

        var response = await HttpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var location = response.Headers.Location?.ToString();
            if (!string.IsNullOrEmpty(location) && location.Split('/').Last() is string id && int.TryParse(id, out var exerciseId))
            {
                return Result<int>.Success(exerciseId);
            }
            return Result<int>.Failure("Exercise created but ID was not returned.");
        }

        return await HandleFailure<int>(response);
    }

    /// Updates an existing exercise definition.
    public async Task<Result> UpdateExercise(Exercise exercise, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, $"api/exercises/{exercise.Id}");
        request.Headers.Add("X-User-Id", userId.ToString());
        request.Content = JsonContent.Create(exercise);

        var response = await HttpClient.SendAsync(request);
        if (response.IsSuccessStatusCode) return Result.Success();

        return await HandleFailure(response);
    }

    /// Removes an exercise definition.
    public async Task<Result> DeleteExercise(int id, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"api/exercises/{id}");
        request.Headers.Add("X-User-Id", userId.ToString());

        var response = await HttpClient.SendAsync(request);
        if (response.IsSuccessStatusCode) return Result.Success();

        return await HandleFailure(response);
    }
}
