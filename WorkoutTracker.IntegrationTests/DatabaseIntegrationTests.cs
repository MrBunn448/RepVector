using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using WorkoutTracker.Logic.Abstractions.Repositories;
using WorkoutTracker.Models;
using Xunit;

namespace WorkoutTracker.IntegrationTests;

/// These tests verify the "End-to-End" flow of the application using a real database (repvectortest).
public class DatabaseIntegrationTests : IClassFixture<WorkoutTrackerWebApplicationFactory>, IAsyncLifetime
{
    private readonly WorkoutTrackerWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly List<int> _testUserIds = new();

    public DatabaseIntegrationTests(WorkoutTrackerWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        // Cleanup: Delete all users created during tests. 
        // We delete workouts first to ensure workout_exercises are removed before exercises.
        using var scope = _factory.Services.CreateScope();
        var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var workoutRepo = scope.ServiceProvider.GetRequiredService<IWorkoutRepository>();

        foreach (var userId in _testUserIds)
        {
            var workouts = await workoutRepo.GetAllByUserIdAsync(userId);
            foreach (var workout in workouts.Where(w => w.UserId == userId))
            {
                await workoutRepo.DeleteAsync(workout.Id);
            }
            await userRepo.DeleteAsync(userId);
        }
    }

    /// Tests the primary user journey: Creating a template, starting a session, and finishing it.
    /// This ensures that the Workouts, Sessions, and History features are correctly wired up.
    [Fact]
    public async Task WorkoutLifecycle_EndToEnd_Works()
    {
        // PHASE 1: PREPARATION
        var testEmail = $"workout_user_{Guid.NewGuid()}@example.com";
        var password = "StrongPassword123!";
        
        await _client.PostAsJsonAsync("/api/auth/register", new { email = testEmail, password = password });
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new { email = testEmail, password = password });
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        var userId = loginResult!.UserId;
        _testUserIds.Add(userId); // Track for cleanup

        // PHASE 2: TEMPLATE CREATION
        var newWorkout = new Workout 
        { 
            Name = "Integration Test Workout", 
            Description = "Testing the full stack",
            UserId = userId
        };
        var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/workouts");
        createRequest.Headers.Add("X-User-Id", userId.ToString());
        createRequest.Content = JsonContent.Create(newWorkout);
        
        var createResponse = await _client.SendAsync(createRequest);
        createResponse.EnsureSuccessStatusCode();
        var workoutIdObj = await createResponse.Content.ReadFromJsonAsync<IdResponse>();
        var workoutId = workoutIdObj!.Id;

        // PHASE 3: LIVE SESSION
        var startRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/workoutsessions/start/{workoutId}");
        startRequest.Headers.Add("X-User-Id", userId.ToString());
        var startResponse = await _client.SendAsync(startRequest);
        startResponse.EnsureSuccessStatusCode();
        var sessionIdObj = await startResponse.Content.ReadFromJsonAsync<IdResponse>();
        var sessionId = sessionIdObj!.Id;

        // PHASE 4: PERFORMANCE LOGGING
        // Create an exercise first to avoid Foreign Key violations
        var exercise = new Exercise { Name = "Test Pushup", Type = "Bodyweight", UserId = userId };
        var exRequest = new HttpRequestMessage(HttpMethod.Post, "/api/exercises");
        exRequest.Headers.Add("X-User-Id", userId.ToString());
        exRequest.Content = JsonContent.Create(exercise);
        var exResponse = await _client.SendAsync(exRequest);
        exResponse.EnsureSuccessStatusCode();
        var exerciseIdObj = await exResponse.Content.ReadFromJsonAsync<IdResponse>();

        var setLog = new WorkoutSetLog
        {
            SessionId = sessionId,
            ExerciseId = exerciseIdObj!.Id, 
            SetNumber = 1,
            Weight = 50,
            Reps = 10,
            Rpe = 8
        };
        var logRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/workoutsessions/{sessionId}/log-set");
        logRequest.Headers.Add("X-User-Id", userId.ToString());
        logRequest.Content = JsonContent.Create(setLog);
        var logResponse = await _client.SendAsync(logRequest);
        logResponse.EnsureSuccessStatusCode();
        
        // PHASE 5: COMPLETION & HISTORY
        var finishRequest = new HttpRequestMessage(HttpMethod.Put, $"/api/workoutsessions/{sessionId}/status");
        finishRequest.Headers.Add("X-User-Id", userId.ToString());
        finishRequest.Content = JsonContent.Create("completed");
        var finishResponse = await _client.SendAsync(finishRequest);
        finishResponse.EnsureSuccessStatusCode();

        var historyRequest = new HttpRequestMessage(HttpMethod.Get, "/api/workoutsessions/user/history");
        historyRequest.Headers.Add("X-User-Id", userId.ToString());
        var historyResponse = await _client.SendAsync(historyRequest);
        historyResponse.EnsureSuccessStatusCode();
        var sessions = await historyResponse.Content.ReadFromJsonAsync<List<WorkoutSession>>();

        // ASSERTIONS
        Assert.NotNull(sessions);
        Assert.Contains(sessions, s => s.Id == sessionId && s.Status == "completed");
    }

    /// Tests the linkage between workouts and custom exercises.
    /// This verifies that the multi-repository orchestration in the Logic layer is correct.
    [Fact]
    public async Task CustomExerciseAndTemplate_Works()
    {
        // Arrange - Register & Login
        var testEmail = $"exercise_user_{Guid.NewGuid()}@example.com"; //New Guiid is for generating a unique email for each test run to avoid conflicts in the database. 
        var password = "StrongPassword123!";
        await _client.PostAsJsonAsync("/api/auth/register", new { email = testEmail, password = password });
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new { email = testEmail, password = password });
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        var userId = loginResult!.UserId;
        _testUserIds.Add(userId); // Track for cleanup

        // Act - Create a Custom Exercise (Private to this user)
        var newExercise = new Exercise 
        { 
            Name = "Deep Squat", 
            Type = "Barbell", 
            UserId = userId 
        };
        var exRequest = new HttpRequestMessage(HttpMethod.Post, "/api/exercises");
        exRequest.Headers.Add("X-User-Id", userId.ToString());
        exRequest.Content = JsonContent.Create(newExercise);
        var exResponse = await _client.SendAsync(exRequest);
        exResponse.EnsureSuccessStatusCode();
        var exerciseIdObj = await exResponse.Content.ReadFromJsonAsync<IdResponse>();
        var exerciseId = exerciseIdObj!.Id;

        // Act - Create a Workout Template that uses the custom exercise created above
        var workout = new Workout 
        { 
            Name = "Leg Power", 
            UserId = userId,
            Exercises = new List<WorkoutExercise>
            {
                new WorkoutExercise { ExerciseId = exerciseId, TargetSets = 3, TargetReps = 5, SortOrder = 1 }
            }
        };
        var wkRequest = new HttpRequestMessage(HttpMethod.Post, "/api/workouts");
        wkRequest.Headers.Add("X-User-Id", userId.ToString());
        wkRequest.Content = JsonContent.Create(workout);
        var wkResponse = await _client.SendAsync(wkRequest);
        wkResponse.EnsureSuccessStatusCode();
        
        // Act - Retrieve the full details of the workout to verify the exercises were linked correctly
        var wkIdObj = await wkResponse.Content.ReadFromJsonAsync<IdResponse>();
        var detailsRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/workouts/{wkIdObj!.Id}/details");
        detailsRequest.Headers.Add("X-User-Id", userId.ToString());
        var detailsResponse = await _client.SendAsync(detailsRequest);
        detailsResponse.EnsureSuccessStatusCode();
        var fetchedWorkout = await detailsResponse.Content.ReadFromJsonAsync<Workout>();

        // Assert
        Assert.NotNull(fetchedWorkout);
        Assert.Single(fetchedWorkout.Exercises);
        // Verify that the exercise details were correctly joined in the query
        Assert.Equal("Deep Squat", fetchedWorkout.Exercises[0].Exercise?.Name);
    }


    //Turn the JSON to c# objects for easier assertions in tests. These are "internal" classes used only within the context of these integration tests to capture specific response shapes from the API.
    private class LoginResponse // To capture the identity of the user after logging in
    {
        public int UserId { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }

    private class IdResponse // A generic "utility" object to catch the ID of any newly created item.
    {
        public int Id { get; set; }
    }
}
