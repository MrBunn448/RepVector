using System.Net.Http.Json;
using System.Text.Json;
using WorkoutTracker.Models;

namespace WorkoutTracker.UI.Services;

/// Client for interacting with the Authentication API.
/// Handles user registration, administrative registration, and login operations.
public class AuthApiClient
{
    private readonly HttpClient _httpClient;

    public AuthApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// Sends a registration request to the API.
    public async Task<(bool Success, int? UserId, string Message)> RegisterAsync(string email, string password)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", new { email, password });

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;

                if (root.TryGetProperty("userId", out var userIdEl) && int.TryParse(userIdEl.ToString(), out var userId))
                {
                    var message = root.TryGetProperty("message", out var msgEl) ? msgEl.GetString() ?? "Registration successful" : "Registration successful";
                    return (true, userId, message);
                }
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;
                var message = root.TryGetProperty("message", out var msgEl) ? msgEl.GetString() ?? "Registration failed" : "Registration failed";
                return (false, null, message);
            }
        }
        catch (Exception ex)
        {
            return (false, null, $"Error: {ex.Message}");
        }

        return (false, null, "Registration failed");
    }

    /// Sends an administrative registration request to the API.
    public async Task<(bool Success, int? UserId, string? Role, string Message)> RegisterAdminAsync(string email, string password, string adminSecret)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register/admin", new { email, password, adminSecret });

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;

                if (root.TryGetProperty("userId", out var userIdEl) && int.TryParse(userIdEl.ToString(), out var userId))
                {
                    var role = root.TryGetProperty("role", out var roleEl) ? roleEl.GetString() : "Admin";
                    var message = root.TryGetProperty("message", out var msgEl) ? msgEl.GetString() ?? "Admin registration successful" : "Admin registration successful";
                    return (true, userId, role, message);
                }
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;
                var message = root.TryGetProperty("message", out var msgEl) ? msgEl.GetString() ?? "Admin registration failed" : "Admin registration failed";
                return (false, null, null, message);
            }
        }
        catch (Exception ex)
        {
            return (false, null, null, $"Error: {ex.Message}");
        }

        return (false, null, null, "Admin registration failed");
    }

    /// Sends a login request to the API.
    public async Task<(bool Success, int? UserId, string? Role, string Message)> LoginAsync(string email, string password)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", new { email, password });

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;

                if (root.TryGetProperty("userId", out var userIdEl) && int.TryParse(userIdEl.ToString(), out var userId))
                {
                    var role = root.TryGetProperty("role", out var roleEl) ? roleEl.GetString() : "User";
                    var message = root.TryGetProperty("message", out var msgEl) ? msgEl.GetString() ?? "Login successful" : "Login successful";
                    return (true, userId, role, message);
                }
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;
                var message = root.TryGetProperty("message", out var msgEl) ? msgEl.GetString() ?? "Login failed" : "Login failed";
                return (false, null, null, message);
            }
        }
        catch (Exception ex)
        {
            return (false, null, null, $"Error: {ex.Message}");
        }

        return (false, null, null, "Login failed");
    }
}
