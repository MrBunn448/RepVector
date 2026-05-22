using System.Net.Http.Json;
using WorkoutTracker.Models;

namespace WorkoutTracker.UI.Services;

public class SessionApiClient
{
    private readonly HttpClient _httpClient;

    /// Initializes a new instance of the SessionApiClient.
    public SessionApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// Retrieves the currently active workout session for a user.
    public async Task<WorkoutSession?> GetActiveSession(int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/WorkoutSessions/user/active");
        request.Headers.Add("X-User-Id", userId.ToString());

        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<WorkoutSession>();
        
        return null;
    }

    /// Retrieves the full workout session history for a user.
    public async Task<List<WorkoutSession>> GetHistory(int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/WorkoutSessions/user/history");
        request.Headers.Add("X-User-Id", userId.ToString());

        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<List<WorkoutSession>>() ?? new();
        
        return new();
    }

    /// Retrieves a specific workout session by its identifier.
    public async Task<WorkoutSession?> GetById(int id, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/WorkoutSessions/{id}");
        request.Headers.Add("X-User-Id", userId.ToString());

        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<WorkoutSession>();
        
        return null;
    }

    /// Starts a new workout session from a template.
    public async Task<(bool success, string? error)> StartSession(int workoutId, int userId, bool cancelExisting = false)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"api/WorkoutSessions/start/{workoutId}?cancelExisting={cancelExisting}");
        request.Headers.Add("X-User-Id", userId.ToString());

        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode) return (true, null);
        
        if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            return (false, "AN_ACTIVE_SESSION_EXISTS");

        return (false, "FAILED_TO_START");
    }

    /// Updates the status of an ongoing workout session.
    public async Task UpdateStatus(int sessionId, string status, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, $"api/WorkoutSessions/{sessionId}/status");
        request.Headers.Add("X-User-Id", userId.ToString());
        request.Content = JsonContent.Create(status);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    /// Records a performance set log for an exercise in a session.
    public async Task LogSet(int sessionId, WorkoutSetLog log, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"api/WorkoutSessions/{sessionId}/log-set");
        request.Headers.Add("X-User-Id", userId.ToString());
        request.Content = JsonContent.Create(log);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    /// Deletes a workout session record.
    public async Task DeleteSession(int sessionId, int userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"api/WorkoutSessions/{sessionId}");
        request.Headers.Add("X-User-Id", userId.ToString());

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}
