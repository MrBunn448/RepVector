using Moq;
using Microsoft.Extensions.Configuration;
using WorkoutTracker.Logic.Abstractions.Repositories;
using WorkoutTracker.Logic.Services;
using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly AuthService _service;
    private const string DefaultAdminSecret = "RepVectorAdmin2026";

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _configurationMock = new Mock<IConfiguration>();
        
        // Mock configuration indexer
        _configurationMock.Setup(c => c["AdminSecretKey"]).Returns(DefaultAdminSecret);
        
        _service = new AuthService(_userRepositoryMock.Object, _configurationMock.Object);
    }

    [Fact]
    public async Task RegisterAdminAsync_WithCorrectSecret_RegistersAsAdmin()
    {
        // Arrange
        var email = "admin@test.com";
        var password = "password123";
        var secret = DefaultAdminSecret;

        _userRepositoryMock
            .Setup(repo => repo.GetByEmailAsync(email))
            .ReturnsAsync((User?)null);

        _userRepositoryMock
            .Setup(repo => repo.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync(1);

        // Act
        var result = await _service.RegisterAdminAsync(email, password, secret);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Admin", result.Value?.Role);
        _userRepositoryMock.Verify(repo => repo.CreateAsync(It.Is<User>(u => u.Role == "Admin")), Times.Once);
    }

    [Fact]
    public async Task RegisterAdminAsync_WithWrongSecret_Fails()
    {
        // Arrange
        var email = "admin@test.com";
        var password = "password123";
        var secret = "WrongSecret";

        // Act
        var result = await _service.RegisterAdminAsync(email, password, secret);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Invalid admin secret.", result.ErrorMessage);
        _userRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAdminAsync_WhenSecretNotConfigured_Fails()
    {
        // Arrange
        _configurationMock.Setup(c => c["AdminSecretKey"]).Returns((string?)null);
        var email = "admin@test.com";
        var password = "password123";

        // Act
        var result = await _service.RegisterAdminAsync(email, password, "SomeSecret");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Admin registration is currently disabled", result.ErrorMessage);
    }
}
