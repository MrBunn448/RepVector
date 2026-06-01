using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using WorkoutTracker.Logic.Abstractions.Repositories;
using WorkoutTracker.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WorkoutTracker.IntegrationTests;

public class WorkoutTrackerWebApplicationFactory : WebApplicationFactory<Program>
{
  //Normally, when you run your API, it "listens" on a network port(like localhost:5123). To talk to it, a browser or

  //In interfration tests, the WebApplicationFactory creates a "Virtual Server." When the test sends a request via the
  //HttpClient, the request never actually leaves your RAM.It is handed directly from the test code to the API code.This
  //makes the tests extremely fast and avoids "Port already in use" errors.
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            // IMPORTANT: This ensures ALL integration tests use the 'repvectortest' database
            config.AddJsonFile("appsettings.json", optional: false);
        });

        builder.ConfigureServices(services =>
        {
        });
    }
}
