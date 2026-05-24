using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using WorkoutTracker.Logic.Abstractions.Repositories;
using WorkoutTracker.Models;

namespace WorkoutTracker.IntegrationTests;

public class WorkoutTrackerWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            // IMPORTANT: This ensures ALL integration tests use the 'repvectortest' database
            // by loading the connection string from the test project's appsettings.json.
            config.AddJsonFile("appsettings.json", optional: false);
        });

        builder.ConfigureServices(services =>
        {
        });
    }
}
