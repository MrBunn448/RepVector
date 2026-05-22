using Moq;
using WorkoutTracker.Logic.Abstractions.Repositories;
using WorkoutTracker.Logic.Abstractions.Services;
using WorkoutTracker.Logic.Services;
using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Tests.Services;

public class WorkoutSessionServiceTests
{
    private readonly Mock<IWorkoutSessionRepository> _sessionRepositoryMock;
    private readonly Mock<IWorkoutRepository> _workoutRepositoryMock;
    private readonly Mock<IAuthorizationService> _authMock;
    private readonly WorkoutSessionService _service;

    public WorkoutSessionServiceTests()
    {
        _sessionRepositoryMock = new Mock<IWorkoutSessionRepository>();
        _workoutRepositoryMock = new Mock<IWorkoutRepository>();
        _authMock = new Mock<IAuthorizationService>();
        _service = new WorkoutSessionService(
            _sessionRepositoryMock.Object, 
            _workoutRepositoryMock.Object,
            _authMock.Object);

        // Default: Allow everything unless specified otherwise
        _authMock.Setup(x => x.CanModifySession(It.IsAny<User>(), It.IsAny<WorkoutSession>()))
                 .Returns(Result.Success());
    }

    [Fact]
    public async Task StartSessionAsync_ReturnsFailure_WhenActiveSessionExists()
    {
        // Arrange
        int userId = 1;
        int workoutId = 1;
        _sessionRepositoryMock.Setup(repo => repo.GetActiveSessionByUserIdAsync(userId))
            .ReturnsAsync(new WorkoutSession { Id = 100, Status = "active" });

        // Act
        var result = await _service.StartSessionAsync(userId, workoutId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("AN_ACTIVE_SESSION_EXISTS", result.ErrorMessage);
    }

    [Fact]
    public async Task StartSessionAsync_CancelsExisting_WhenRequested()
    {
        // Arrange
        int userId = 1;
        int workoutId = 1;
        var existingSession = new WorkoutSession { Id = 100, Status = "active" };
        _sessionRepositoryMock.Setup(repo => repo.GetActiveSessionByUserIdAsync(userId))
            .ReturnsAsync(existingSession);

        var workoutTemplate = new Workout { Id = 1, Name = "Leg Day" };
        _workoutRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(workoutTemplate);

        // Act
        var result = await _service.StartSessionAsync(userId, workoutId, cancelExisting: true);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("cancelled", existingSession.Status);
        Assert.NotNull(existingSession.FinishedAt);
        _sessionRepositoryMock.Verify(repo => repo.UpdateAsync(existingSession), Times.Once);
        _sessionRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<WorkoutSession>()), Times.Once);
    }

    [Fact]
    public async Task UpdateSessionStatusAsync_CalculatesDuration_OnCompletion()
    {
        // Arrange
        var editor = new User { Id = 1, Role = "User" };
        var startedAt = DateTime.UtcNow.AddMinutes(-45);
        var session = new WorkoutSession { Id = 1, UserId = 1, StartedAt = startedAt, TotalSeconds = 0 };

        _sessionRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(session);

        // Act
        var result = await _service.UpdateSessionStatusAsync(1, "completed", editor);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("completed", session.Status);
        Assert.InRange(session.TotalSeconds, 2690, 2710); // Around 2700 seconds (45 min)
        _sessionRepositoryMock.Verify(repo => repo.UpdateAsync(session), Times.Once);
    }

    [Fact]
    public async Task SaveSetLogAsync_ReturnsFailure_WhenUnauthorized()
    {
        // Arrange
        var user = new User { Id = 2, Role = "User" };
        var session = new WorkoutSession { Id = 1, UserId = 1 }; // Owned by User 1
        var log = new WorkoutSetLog { SessionId = 1 };

        _sessionRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(session);
        _authMock.Setup(x => x.CanModifySession(user, session))
                 .Returns(Result.Forbidden("Access denied"));

        // Act
        var result = await _service.SaveSetLogAsync(log, user);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Access denied", result.ErrorMessage);
    }
}
