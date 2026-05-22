using Moq;
using WorkoutTracker.Logic.Abstractions.Repositories;
using WorkoutTracker.Logic.Abstractions.Services;
using WorkoutTracker.Logic.Services;
using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Tests.Services;

public class WorkoutExerciseServiceTests
{
    private readonly Mock<IWorkoutExerciseRepository> _weRepoMock;
    private readonly Mock<IWorkoutRepository> _workoutRepoMock;
    private readonly Mock<IAuthorizationService> _authMock;
    private readonly WorkoutExerciseService _service;

    public WorkoutExerciseServiceTests()
    {
        _weRepoMock = new Mock<IWorkoutExerciseRepository>();
        _workoutRepoMock = new Mock<IWorkoutRepository>();
        _authMock = new Mock<IAuthorizationService>();
        _service = new WorkoutExerciseService(_weRepoMock.Object, _workoutRepoMock.Object, _authMock.Object);

        // Default: Allow everything unless specified otherwise
        _authMock.Setup(x => x.CanModifyWorkout(It.IsAny<User>(), It.IsAny<Workout>()))
                 .Returns(Result.Success());
    }

    [Fact]
    public async Task AddAsync_ReturnsFailure_WhenUserAddsToAdminWorkout()
    {
        // Arrange
        var user = new User { Id = 10, Role = "User" };
        var workout = new Workout { Id = 1, IsPredefined = true };
        var we = new WorkoutExercise { WorkoutId = 1 };

        _workoutRepoMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(workout);
        _authMock.Setup(x => x.CanModifyWorkout(user, workout))
                 .Returns(Result.Forbidden("Access denied"));

        // Act
        var result = await _service.AddAsync(we, user);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Access denied", result.ErrorMessage);
    }

    [Fact]
    public async Task UpdateAsync_CallsRepository_WhenUserIsOwner()
    {
        // Arrange
        var user = new User { Id = 10, Role = "User" };
        var workout = new Workout { Id = 1, UserId = 10, IsPredefined = false };
        var we = new WorkoutExercise { Id = 50, WorkoutId = 1 };

        _weRepoMock.Setup(repo => repo.GetByIdAsync(50)).ReturnsAsync(we);
        _workoutRepoMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(workout);

        // Act
        var result = await _service.UpdateAsync(we, user);

        // Assert
        Assert.True(result.IsSuccess);
        _weRepoMock.Verify(repo => repo.UpdateAsync(we), Times.Once);
    }
}
