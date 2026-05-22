using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using WorkoutTracker.Logic.Abstractions.Repositories;
using WorkoutTracker.Models;

namespace WorkoutTracker.IntegrationTests;

public class WorkoutTrackerWebApplicationFactory : WebApplicationFactory<Program>
{
    public Mock<IWorkoutRepository> WorkoutRepositoryMock { get; } = new();
    public Mock<IExerciseRepository> ExerciseRepositoryMock { get; } = new();
    public Mock<IUserRepository> UserRepositoryMock { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove real repository registrations
            var descriptors = services.Where(
                d => d.ServiceType == typeof(IWorkoutRepository) ||
                     d.ServiceType == typeof(IExerciseRepository) ||
                     d.ServiceType == typeof(IUserRepository)).ToList();

            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }

            // Add mocks
            services.AddScoped(_ => WorkoutRepositoryMock.Object);
            services.AddScoped(_ => ExerciseRepositoryMock.Object);
            services.AddScoped(_ => UserRepositoryMock.Object);
        });
    }
}
