using System.Net.Http.Json;
using WorkoutTracker.Models;

namespace WorkoutTracker.UI.Services;

/// Manages workout sessions and performance logging.
/// Inherits from BaseApiClient for standardized error handling.
public class SessionApiClient(HttpClient httpClient) : BaseApiClient(httpClient)
{
    /// Retrieves the currently active workout session for a user.
    public async Task<Result<WorkoutSession>> GetActiveSession(int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/WorkoutSessions/user/active");
        request.Headers.Add("X-User-Id", userId.ToString());

        var response = await HttpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<WorkoutSession>();
            return data != null ? Result<WorkoutSession>.Success(data) : Result<WorkoutSession>.NotFound();
        }
        
        return await HandleFailure<WorkoutSession>(response);
    }

    /// Retrieves the full workout session history for a user.
    public async Task<Result<List<WorkoutSession>>> GetHistory(int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/WorkoutSessions/user/history");
        request.Headers.Add("X-User-Id", userId.ToString());

        var response = await HttpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<List<WorkoutSession>>() ?? new();
            return Result<List<WorkoutSession>>.Success(data);
        }
        
        return await HandleFailure<List<WorkoutSession>>(response);
    }

    /// Retrieves all performance logs for a specific session.
    public async Task<Result<List<WorkoutSetLog>>> GetSessionLogs(int sessionId, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/WorkoutSessions/{sessionId}/logs");
        request.Headers.Add("X-User-Id", userId.ToString());

        var response = await HttpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<List<WorkoutSetLog>>() ?? new();
            return Result<List<WorkoutSetLog>>.Success(data);
        }

        return await HandleFailure<List<WorkoutSetLog>>(response);
    }

    /// Retrieves a specific workout session by its identifier.
    public async Task<Result<WorkoutSession>> GetById(int id, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/WorkoutSessions/{id}");
        request.Headers.Add("X-User-Id", userId.ToString());

        var response = await HttpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<WorkoutSession>();
            return data != null ? Result<WorkoutSession>.Success(data) : Result<WorkoutSession>.NotFound();
        }
        
        return await HandleFailure<WorkoutSession>(response);
    }

    /// Starts a new workout session from a template.
    public async Task<(bool success, string? error)> StartSession(int workoutId, int userId, bool cancelExisting = false)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"api/WorkoutSessions/start/{workoutId}?cancelExisting={cancelExisting}");
        request.Headers.Add("X-User-Id", userId.ToString());

        var response = await HttpClient.SendAsync(request);
        if (response.IsSuccessStatusCode) return (true, null);
        
        if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            return (false, "AN_ACTIVE_SESSION_EXISTS");

        return (false, await TryGetErrorMessage(response));
    }

    /// Updates the status of an ongoing workout session.
    public async Task<Result> UpdateStatus(int sessionId, string status, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, $"api/WorkoutSessions/{sessionId}/status");
        request.Headers.Add("X-User-Id", userId.ToString());
        request.Content = JsonContent.Create(status);

        var response = await HttpClient.SendAsync(request);
        if (response.IsSuccessStatusCode) return Result.Success();

        return await HandleFailure(response);
    }

    /// Records a performance set log for an exercise in a session.
    public async Task<Result<WorkoutSetLog>> LogSet(int sessionId, WorkoutSetLog log, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"api/WorkoutSessions/{sessionId}/log-set");
        request.Headers.Add("X-User-Id", userId.ToString());
        request.Content = JsonContent.Create(log);

        var response = await HttpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<WorkoutSetLog>();
            return data != null ? Result<WorkoutSetLog>.Success(data) : Result<WorkoutSetLog>.Failure("Failed to deserialize log.");
        }

        return await HandleFailure<WorkoutSetLog>(response);
    }

    /// Deletes a specific set log.
    public async Task<Result> DeleteSet(int sessionId, int logId, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"api/WorkoutSessions/{sessionId}/sets/{logId}");
        request.Headers.Add("X-User-Id", userId.ToString());

        var response = await HttpClient.SendAsync(request);
        if (response.IsSuccessStatusCode) return Result.Success();

        return await HandleFailure(response);
    }

    /// Deletes all logs for a specific exercise in a session.
    public async Task<Result> DeleteExerciseLogs(int sessionId, int exerciseId, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"api/WorkoutSessions/{sessionId}/exercises/{exerciseId}/logs");
        request.Headers.Add("X-User-Id", userId.ToString());

        var response = await HttpClient.SendAsync(request);
        if (response.IsSuccessStatusCode) return Result.Success();

        return await HandleFailure(response);
    }

    /// Deletes a workout session record.
    public async Task<Result> DeleteSession(int sessionId, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"api/WorkoutSessions/{sessionId}");
        request.Headers.Add("X-User-Id", userId.ToString());

        var response = await HttpClient.SendAsync(request);
        if (response.IsSuccessStatusCode) return Result.Success();

        return await HandleFailure(response);
    }
}
