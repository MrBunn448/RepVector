using WorkoutTracker.DAL;
using WorkoutTracker.Logic.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' not found.");

builder.Services.AddSingleton(new DbConnectionFactory(connectionString));

// Dependecy injection, by initiating the WorkoutRepository with the DbConnectionFactory, it will automatically resolve the dependency when WorkoutRepository is requested.
// so instead of manually saying new class etc., you can just add the services to the container and let the framework handle the instantiation and dependency resolution.
// this way you can control the lifetime of the services.
// With DI you can define:

//AddSingleton   → one instance for entire app
//AddScoped      → one per HTTP request
//AddTransient   → new every time

builder.Services.AddScoped<IWorkoutRepository, WorkoutRepository>();
builder.Services.AddScoped<WorkoutService>(); 

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();