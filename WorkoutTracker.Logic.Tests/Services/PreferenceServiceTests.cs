using Moq;
using WorkoutTracker.Logic.Abstractions.Repositories;
using WorkoutTracker.Logic.Abstractions.Services;
using WorkoutTracker.Logic.Services;
using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Tests.Services;

public class PreferenceServiceTests
{
    private readonly Mock<IUserPreferencesRepository> _prefRepositoryMock;
    private readonly Mock<IAuthorizationService> _authMock;
    private readonly PreferenceService _service;

    public PreferenceServiceTests()
    {
        _prefRepositoryMock = new Mock<IUserPreferencesRepository>();
        _authMock = new Mock<IAuthorizationService>();
        _service = new PreferenceService(_prefRepositoryMock.Object, _authMock.Object);

        // Default: Allow everything unless specified otherwise
        _authMock.Setup(x => x.CanModifyPreference(It.IsAny<User>(), It.IsAny<int>()))
                 .Returns(Result.Success());
    }

    [Theory]
    [InlineData(100, "KG", "100.0 Kg")]
    [InlineData(100, "LBS", "220.5 Lbs")]
    [InlineData(75.5, "KG", "75.5 Kg")]
    [InlineData(75.5, "LBS", "166.4 Lbs")]
    public void FormatWeight_ReturnsCorrectString(decimal weight, string unit, string expected)
    {
        // Act
        var actual = _service.FormatWeight(weight, unit);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(10, "Km", "10.0 Km")]
    [InlineData(10, "Miles", "6.2 Miles")]
    public void FormatDistance_ReturnsCorrectString(decimal distance, string unit, string expected)
    {
        // Act
        var actual = _service.FormatDistance(distance, unit);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task SavePreferencesAsync_CallsRepository()
    {
        // Arrange
        var user = new User { Id = 1, Role = "User" };
        var prefs = new UserPreferences { UserId = 1, WeightUnit = "KG" };

        // Act
        var result = await _service.SavePreferencesAsync(prefs, user);

        // Assert
        Assert.True(result.IsSuccess);
        _prefRepositoryMock.Verify(repo => repo.CreateOrUpdateAsync(prefs), Times.Once);
    }

    [Fact]
    public async Task SavePreferencesAsync_ReturnsFailure_WhenUnauthorized()
    {
        // Arrange
        var user = new User { Id = 2, Role = "User" };
        var prefs = new UserPreferences { UserId = 1, WeightUnit = "KG" };

        _authMock.Setup(x => x.CanModifyPreference(user, 1))
                 .Returns(Result.Forbidden("Access denied"));

        // Act
        var result = await _service.SavePreferencesAsync(prefs, user);

        // Assert
        Assert.False(result.IsSuccess);
        _prefRepositoryMock.Verify(repo => repo.CreateOrUpdateAsync(prefs), Times.Never);
    }
}
