using System.Net;
using System.Net.Http.Json;
using Moq;
using WorkoutTracker.Models;
using Xunit;

namespace WorkoutTracker.IntegrationTests;

public class WorkoutApiTests : IClassFixture<WorkoutTrackerWebApplicationFactory>
{
    private readonly WorkoutTrackerWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public WorkoutApiTests(WorkoutTrackerWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetWorkouts_ReturnsOk_WithWorkouts()
    {
        // Arrange
        var userId = 1;
        var mockUser = new User { Id = userId, Role = "User" };
        _factory.UserRepositoryMock
            .Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(mockUser);

        var mockWorkouts = new List<Workout>
        {
            new Workout { Id = 1, Name = "Test Workout", UserId = userId }
        };
        _factory.WorkoutRepositoryMock
            .Setup(repo => repo.GetAllByUserIdAsync(userId))
            .ReturnsAsync(mockWorkouts);

        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/workouts");
        request.Headers.Add("X-User-Id", userId.ToString());
        var response = await _client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var workouts = await response.Content.ReadFromJsonAsync<List<Workout>>();
        Assert.NotNull(workouts);
        Assert.Single(workouts);
        Assert.Equal("Test Workout", workouts[0].Name);
    }

    [Fact]
    public async Task GetWorkoutById_ReturnsNotFound_WhenWorkoutDoesNotExist()
    {
        // Arrange
        var userId = 1;
        var workoutId = 999;
        var mockUser = new User { Id = userId, Role = "User" };
        _factory.UserRepositoryMock
            .Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(mockUser);

        _factory.WorkoutRepositoryMock
            .Setup(repo => repo.GetByIdAsync(workoutId))
            .ReturnsAsync((Workout?)null);

        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/workouts/{workoutId}");
        request.Headers.Add("X-User-Id", userId.ToString());
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
