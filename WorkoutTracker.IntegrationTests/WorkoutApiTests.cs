using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using WorkoutTracker.Logic.Abstractions.Repositories;
using WorkoutTracker.Models;
using Xunit;

namespace WorkoutTracker.IntegrationTests;

public class WorkoutApiTests : IClassFixture<WorkoutTrackerWebApplicationFactory>, IAsyncLifetime
{
    private readonly WorkoutTrackerWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly List<int> _testUserIds = new();

    public WorkoutApiTests(WorkoutTrackerWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        // Cleanup: Delete all users created during tests. 
        using var scope = _factory.Services.CreateScope(); // 
        var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var workoutRepo = scope.ServiceProvider.GetRequiredService<IWorkoutRepository>();

        foreach (var userId in _testUserIds)
        {
            // Delete workouts first to satisfy foreign key constraints for workout_exercises
            var workouts = await workoutRepo.GetAllByUserIdAsync(userId);
            foreach (var workout in workouts.Where(w => w.UserId == userId))
            {
                await workoutRepo.DeleteAsync(workout.Id);
            }
            await userRepo.DeleteAsync(userId);
        }
    }

    [Fact] // Fact, marks it as a test method that should be run by the test runner.
    public async Task GetWorkouts_ReturnsOk_WithWorkouts()
    {
        // step 1: PREPARATION (Dynamic User)
        var userId = await CreateTestUserAndLogin();

        // step 2: DATA SETUP (Dynamic Workout)
        var workoutName = $"API Test Workout {Guid.NewGuid()}";
        await CreateTestWorkout(userId, workoutName);

        // step 3: THE ACTUAL TEST
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/workouts");
        request.Headers.Add("X-User-Id", userId.ToString());
        var response = await _client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var workouts = await response.Content.ReadFromJsonAsync<List<Workout>>();
        Assert.NotNull(workouts);
        Assert.Contains(workouts, w => w.Name == workoutName);
    }

    [Fact]
    public async Task GetWorkoutById_ReturnsNotFound_WhenWorkoutDoesNotExist()
    {
        // Arrange - Use a real user but an ID that is guaranteed not to exist (like a random negative number or very high)
        var userId = await CreateTestUserAndLogin();
        var workoutId = 999999; 

        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/workouts/{workoutId}");
        request.Headers.Add("X-User-Id", userId.ToString());
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // HELPER METHODS FOR DYNAMIC FLOW (??)

    private async Task<int> CreateTestUserAndLogin()
    {
        var email = $"user_{Guid.NewGuid()}@example.com";
        var password = "Password123!";

        // Register
        await _client.PostAsJsonAsync("/api/auth/register", new { email, password });
        
        // Login
        var response = await _client.PostAsJsonAsync("/api/auth/login", new { email, password });
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        
        var userId = result!.UserId;
        _testUserIds.Add(userId);
        return userId;
    }

    private async Task<int> CreateTestWorkout(int userId, string name)
    {
        var workout = new Workout { Name = name, UserId = userId };
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/workouts");
        request.Headers.Add("X-User-Id", userId.ToString());
        request.Content = JsonContent.Create(workout);
        
        var response = await _client.SendAsync(request);
        var result = await response.Content.ReadFromJsonAsync<IdResponse>();
        return result!.Id;
    }

    private class LoginResponse
    {
        public int UserId { get; set; }
    }

    private class IdResponse
    {
        public int Id { get; set; }
    }
}
