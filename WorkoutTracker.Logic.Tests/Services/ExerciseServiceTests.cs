using Moq;
using WorkoutTracker.Logic.Abstractions.Repositories;
using WorkoutTracker.Logic.Abstractions.Services;
using WorkoutTracker.Logic.Services;
using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Tests.Services;

public class ExerciseServiceTests
{
    private readonly Mock<IExerciseRepository> _exerciseRepositoryMock;
    private readonly Mock<IAuthorizationService> _authMock;
    private readonly ExerciseService _service;

    public ExerciseServiceTests()
    {
        _exerciseRepositoryMock = new Mock<IExerciseRepository>();
        _authMock = new Mock<IAuthorizationService>();
        _service = new ExerciseService(_exerciseRepositoryMock.Object, _authMock.Object);

        // Default: Allow everything unless specified otherwise
        _authMock.Setup(x => x.CanModifyExercise(It.IsAny<User>(), It.IsAny<Exercise>()))
                 .Returns(Result.Success());
    }

    [Fact]
    public async Task GetAllAsync_CallsRepositoryWithUserId()
    {
        // Arrange
        int userId = 1;
        _exerciseRepositoryMock.Setup(repo => repo.GetAllAsync(userId)).ReturnsAsync(new List<Exercise>());

        // Act
        await _service.GetAllAsync(userId);

        // Assert
        _exerciseRepositoryMock.Verify(repo => repo.GetAllAsync(userId), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_SetsUserId_ForNonPredefinedExercise()
    {
        // Arrange
        var creator = new User { Id = 10, Role = "User" };
        var exercise = new Exercise { Name = "Bicep Curl", IsPredefined = false };

        // Act
        var result = await _service.CreateAsync(exercise, creator);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(creator.Id, exercise.UserId);
        _exerciseRepositoryMock.Verify(repo => repo.CreateAsync(exercise), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFailure_WhenNonAdminCreatesPredefinedExercise()
    {
        // Arrange
        var creator = new User { Id = 10, Role = "User" };
        var exercise = new Exercise { Name = "Bench Press", IsPredefined = true };
        
        _authMock.Setup(x => x.CanModifyExercise(creator, exercise))
                 .Returns(Result.Forbidden("Only admins can create predefined exercises."));

        // Act
        var result = await _service.CreateAsync(exercise, creator);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Only admins can create predefined movements.", result.ErrorMessage);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFailure_WhenUserEditsAdminExercise()
    {
        // Arrange
        var editor = new User { Id = 10, Role = "User" };
        var existingExercise = new Exercise { Id = 1, IsPredefined = true };
        var updatedExercise = new Exercise { Id = 1, Name = "Modified Name" };

        _exerciseRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingExercise);
        _authMock.Setup(x => x.CanModifyExercise(editor, existingExercise))
                 .Returns(Result.Forbidden("Cannot modify predefined exercise."));

        // Act
        var result = await _service.UpdateAsync(updatedExercise, editor);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Cannot modify predefined exercise.", result.ErrorMessage);
    }

    [Fact]
    public async Task DeleteAsync_CallsRepository_WhenUserIsOwner()
    {
        // Arrange
        var deleter = new User { Id = 10, Role = "User" };
        var existingExercise = new Exercise { Id = 1, UserId = 10, IsPredefined = false };

        _exerciseRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingExercise);

        // Act
        var result = await _service.DeleteAsync(1, deleter);

        // Assert
        Assert.True(result.IsSuccess);
        _exerciseRepositoryMock.Verify(repo => repo.DeleteAsync(1), Times.Once);
    }
}
