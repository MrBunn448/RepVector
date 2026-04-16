using WorkoutTracker.DAL;
using WorkoutTracker.Logic.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' not found.");

builder.Services.AddSingleton(new DbConnectionFactory(connectionString));

builder.Services.AddScoped<WorkoutRepository>();
builder.Services.AddScoped<WorkoutService>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();