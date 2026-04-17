using Moq;
using WorkoutTracker.DAL;
using WorkoutTracker.Logic.Services;
using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Tests.Services;

/// <summary>
/// Unit tests for WorkoutService. Each test follows the Arrange / Act / Assert pattern
/// and uses a mocked IWorkoutRepository so tests are deterministic and fast.
/// </summary>
public class WorkoutServiceTests
{
    // A mock of the repository dependency used by the service under test.
    private readonly Mock<IWorkoutRepository> _repositoryMock;

    // The service instance we are testing.
    private readonly WorkoutService _service;

    public WorkoutServiceTests()
    {
        _repositoryMock = new Mock<IWorkoutRepository>();
        _service = new WorkoutService(_repositoryMock.Object);
    }

    /// <summary>
    /// Verifies that GetAllAsync returns the list of workouts provided by the repository.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ReturnsListOfWorkouts()
    {
        // Arrange: prepare the expected workouts that the mock repository will return.
        var expectedWorkouts = new List<Workout>
        {
            new Workout { Id = 1, Name = "Push Day" },
            new Workout { Id = 2, Name = "Pull Day" }
        };

        _repositoryMock
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(expectedWorkouts);

        // Act: invoke the service method under test.
        var actual = await _service.GetAllAsync();

        // Assert: verify the returned collection matches expectations and the repository was called once.
        Assert.NotNull(actual);
        Assert.Equal(2, actual.Count);
        Assert.Equal("Push Day", actual[0].Name);

        _repositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    /// <summary>
    /// When the repository returns a workout with its exercises, the service should return the same details.
    /// </summary>
    [Fact]
    public async Task GetWorkoutDetailsAsync_ReturnsWorkout_WhenFound()
    {
        // Arrange: create a workout with exercises that the mock will return for id 1.
        var expectedWorkout = new Workout
        {
            Id = 1,
            Name = "Leg Day",
            Exercises = new List<Exercise>
            {
                new Exercise { Name = "Squat", Sets = 4, Reps = 8 }
            }
        };

        _repositoryMock
            .Setup(repo => repo.GetWorkoutWithExercises(1))
            .Returns(expectedWorkout);

        // Act
        var actual = await _service.GetWorkoutDetailsAsync(1);

        // Assert: ensure the service returned the expected workout and exercises, and that the repo was queried once.
        Assert.NotNull(actual);
        Assert.Equal("Leg Day", actual.Name);
        Assert.Single(actual.Exercises);

        _repositoryMock.Verify(
            repo => repo.GetWorkoutWithExercises(1),
            Times.Once
        );
    }

    /// <summary>
    /// When the repository returns null for a missing workout, the service should also return null.
    /// </summary>
    [Fact]
    public async Task GetWorkoutDetailsAsync_ReturnsNull_WhenNotFound()
    {
        // Arrange: configure the mock to return null for any id.
        _repositoryMock
            .Setup(repo => repo.GetWorkoutWithExercises(It.IsAny<int>()))
            .Returns((Workout?)null);

        // Act
        var actual = await _service.GetWorkoutDetailsAsync(999);

        // Assert
        Assert.Null(actual);
    }
}
