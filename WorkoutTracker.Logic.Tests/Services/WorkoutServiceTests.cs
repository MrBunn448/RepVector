using Moq;
using WorkoutTracker.Logic.Abstractions.Repositories;
using WorkoutTracker.Logic.Abstractions.Services;
using WorkoutTracker.Logic.Services;
using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Tests.Services;

public class WorkoutServiceTests
{
    // A mock of the repository dependency used by the service under test.
    private readonly Mock<IWorkoutRepository> _repositoryMock;
    private readonly Mock<IWorkoutExerciseRepository> _workoutExerciseRepositoryMock;
    private readonly Mock<IExerciseRepository> _exerciseRepositoryMock;
    private readonly Mock<IAuthorizationService> _authMock;

    private readonly WorkoutService _service;

    public WorkoutServiceTests()
    {
        _repositoryMock = new Mock<IWorkoutRepository>();
        _workoutExerciseRepositoryMock = new Mock<IWorkoutExerciseRepository>();
        _exerciseRepositoryMock = new Mock<IExerciseRepository>();
        _authMock = new Mock<IAuthorizationService>();
        
        _service = new WorkoutService(
            _repositoryMock.Object, 
            _workoutExerciseRepositoryMock.Object,
            _exerciseRepositoryMock.Object,
            _authMock.Object);
    }

    /// Verifies that GetAllByUserIdAsync returns the list of workouts provided by the repository.
    [Fact]
    public async Task GetAllByUserIdAsync_ReturnsListOfWorkouts()
    {
        // Arrange: prepare the expected workouts that the mock repository will return.
        var expectedWorkouts = new List<Workout>
        {
            new Workout { Id = 1, UserId = 1, Name = "Push Day", CreatedAt = DateTime.UtcNow, IsPredefined = false },
            new Workout { Id = 2, UserId = 1, Name = "Pull Day", CreatedAt = DateTime.UtcNow, IsPredefined = false }
        };

        _repositoryMock
            .Setup(repo => repo.GetAllByUserIdAsync(1))
            .ReturnsAsync(expectedWorkouts);

        // Act: invoke the service method under test.
        var actual = await _service.GetAllByUserIdAsync(1);

        // Assert: verify the returned collection matches expectations and the repository was called once.
        Assert.NotNull(actual);
        Assert.Equal(2, actual.Count);
        Assert.Equal("Push Day", actual[0].Name);

        _repositoryMock.Verify(repo => repo.GetAllByUserIdAsync(1), Times.Once);
    }

    /// When the repository returns a workout with its exercises, the service should return the same details.
    [Fact]
    public async Task GetWorkoutDetailsAsync_ReturnsWorkout_WhenFound()
    {
        // Arrange: create a workout with exercises that the mock will return for id 1.
        var expectedWorkout = new Workout
        {
            Id = 1,
            UserId = 1,
            Name = "Leg Day",
            CreatedAt = DateTime.UtcNow,
            Exercises = new List<WorkoutExercise>
            {
                new WorkoutExercise 
                { 
                    Id = 1,
                    WorkoutId = 1,
                    ExerciseId = 1,
                    TargetSets = 4, 
                    TargetReps = 8,
                    SortOrder = 1,
                    Exercise = new Exercise { Id = 1, Name = "Squat", CreatedAt = DateTime.UtcNow }
                }
            }
        };

        _repositoryMock
            .Setup(repo => repo.GetWorkoutWithExercisesAsync(1))
            .ReturnsAsync(expectedWorkout);

        // Act
        var actual = await _service.GetWorkoutDetailsAsync(1);

        // Assert: ensure the service returned the expected workout and exercises, and that the repo was queried once.
        Assert.NotNull(actual);
        Assert.Equal("Leg Day", actual.Name);
        Assert.Single(actual.Exercises);

        _repositoryMock.Verify(
            repo => repo.GetWorkoutWithExercisesAsync(1),
            Times.Once
        );
    }

    /// When the repository returns null for a missing workout, the service should also return null.
    [Fact]
    public async Task GetWorkoutDetailsAsync_ReturnsNull_WhenNotFound()
    {
        // Arrange: configure the mock to return null for any id.
        _repositoryMock
            .Setup(repo => repo.GetWorkoutWithExercisesAsync(It.IsAny<int>()))
            .ReturnsAsync((Workout?)null);

        // Act
        var actual = await _service.GetWorkoutDetailsAsync(999);

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public async Task UpdateWorkoutAsync_SyncsExercisesCorrectly()
    {
        // Arrange
        var editor = new User { Id = 1, Role = "User" };
        var existingWorkout = new Workout
        {
            Id = 1,
            UserId = 1,
            Exercises = new List<WorkoutExercise>
            {
                new WorkoutExercise { Id = 10, ExerciseId = 5, TargetSets = 3, TargetRpe = 8 }, // Existing
                new WorkoutExercise { Id = 11, ExerciseId = 6, TargetSets = 3, TargetRpe = null }  // To be deleted
            }
        };

        var updatedWorkout = new Workout
        {
            Id = 1,
            UserId = 1,
            Exercises = new List<WorkoutExercise>
            {
                new WorkoutExercise { Id = 10, ExerciseId = 5, TargetSets = 4, TargetRpe = 9 }, // Updated
                new WorkoutExercise { Id = 0, ExerciseId = 7, TargetSets = 3, TargetRpe = 7 }   // New
            }
        };

        _repositoryMock.Setup(repo => repo.GetWorkoutWithExercisesAsync(1)).ReturnsAsync(existingWorkout);
        _repositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Workout>())).Returns(Task.CompletedTask);
        _authMock.Setup(a => a.CanModifyWorkout(editor, existingWorkout)).Returns(Result.Success());

        // Act
        var result = await _service.UpdateWorkoutAsync(updatedWorkout, editor);

        // Assert
        Assert.True(result.IsSuccess);
        _workoutExerciseRepositoryMock.Verify(repo => repo.DeleteAsync(11), Times.Once);
        _workoutExerciseRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<WorkoutExercise>(we => we.Id == 10 && we.TargetSets == 4 && we.TargetRpe == 9)), Times.Once);
        _workoutExerciseRepositoryMock.Verify(repo => repo.CreateAsync(It.Is<WorkoutExercise>(we => we.ExerciseId == 7 && we.TargetRpe == 7)), Times.Once);
    }

    [Fact]
    public async Task CreateWorkoutAsync_ReturnsFailure_WhenPredefinedWorkoutHasPersonalExercise()
    {
        // Arrange
        var admin = new User { Id = 1, Role = "Admin" };
        var workout = new Workout
        {
            IsPredefined = true,
            Name = "Global Template",
            Exercises = new List<WorkoutExercise>
            {
                new WorkoutExercise { ExerciseId = 5 }
            }
        };

        var personalExercise = new Exercise { Id = 5, IsPredefined = false, Name = "My Custom Squat" };
        _exerciseRepositoryMock.Setup(repo => repo.GetByIdAsync(5)).ReturnsAsync(personalExercise);

        // Act
        var result = await _service.CreateWorkoutAsync(workout, admin);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("cannot contain personal exercise", result.ErrorMessage);
    }
}
