# Integration Testing Guide

This document describes the integration testing strategy and setup for the RepVector project.

## Overview
Integration tests in RepVector are designed to verify the correct interaction between the API controllers, business services, and the authentication layer. They use the `Microsoft.AspNetCore.Mvc.Testing` library to host the application in-memory and perform HTTP requests against its endpoints.

## Project Structure
- **WorkoutTracker.IntegrationTests**: Contains the integration test suite.
  - `WorkoutTrackerWebApplicationFactory.cs`: A custom `WebApplicationFactory` that overrides service registrations to inject mocks or test-specific configurations.
  - `WorkoutApiTests.cs`: Contains end-to-end tests for the Workout API endpoints.

## Testing Strategy

### In-Memory Web Hosting
We use `WebApplicationFactory<Program>` to bootstrap the API in a test environment. This allows us to test the entire request pipeline, including:
- Routing
- Model Binding
- Action Filters (like authentication checks)
- Serialization/Deserialization

### Dependency Isolation
While these are integration tests, we currently use **Moq** to isolate the Data Access Layer (DAL). This ensures that:
- Tests do not require a live MySQL database.
- Tests are fast and deterministic.
- We can easily simulate database errors or specific data scenarios.

### Authentication Mocking
The API requires an `X-User-Id` header for most operations. Our integration tests:
1. Provide a mock `IUserRepository` that returns a valid user for a given ID.
2. Include the `X-User-Id` header in all test requests.

## Running Tests
To execute the integration tests, use the .NET CLI from the root directory:

```powershell
dotnet test WorkoutTracker.IntegrationTests/WorkoutTracker.IntegrationTests.csproj
```

## Creating New Tests
When adding new integration tests:
1. **Fixture**: Use `IClassFixture<WorkoutTrackerWebApplicationFactory>` to share the test server across multiple tests.
2. **Arrange**: Set up the necessary mock behaviors in the `WorkoutTrackerWebApplicationFactory`.
3. **Act**: Use the `HttpClient` provided by the factory to send requests.
4. **Assert**: Verify the HTTP status code and the response body content.

Example:
```csharp
[Fact]
public async Task GetWorkouts_ReturnsOk()
{
    // Arrange
    var userId = 1;
    _factory.UserRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(new User { Id = userId });
    
    // Act
    var request = new HttpRequestMessage(HttpMethod.Get, "/api/workouts");
    request.Headers.Add("X-User-Id", userId.ToString());
    var response = await _client.SendAsync(request);
    
    // Assert
    response.EnsureSuccessStatusCode();
}
```
