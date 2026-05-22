using WorkoutTracker.DAL;
using WorkoutTracker.DAL.Repositories;
using WorkoutTracker.Logic.Abstractions.Repositories;
using WorkoutTracker.Logic.Abstractions.Services;
using WorkoutTracker.Logic.Infrastructure;
using WorkoutTracker.Logic.Services;
using WorkoutTracker.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment.EnvironmentName;
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' not found.");

builder.Services.AddSingleton(new DbConnectionFactory(connectionString));

// Infrastructure
builder.Services.AddScoped<UserContext>();

// Repositories (SOLID - Abstractions)
builder.Services.AddScoped<IWorkoutRepository, WorkoutRepository>();
builder.Services.AddScoped<IExerciseRepository, ExerciseRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IWorkoutSessionRepository, WorkoutSessionRepository>();
builder.Services.AddScoped<IWorkoutExerciseRepository, WorkoutExerciseRepository>();
builder.Services.AddScoped<IMuscleGroupRepository, MuscleGroupRepository>();
builder.Services.AddScoped<IUserPreferencesRepository, UserPreferencesRepository>();

// Services (Business Logic)
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();
builder.Services.AddScoped<IWorkoutService, WorkoutService>();
builder.Services.AddScoped<IExerciseService, ExerciseService>();
builder.Services.AddScoped<IWorkoutSessionService, WorkoutSessionService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IWorkoutExerciseService, WorkoutExerciseService>();
builder.Services.AddScoped<IPreferenceService, PreferenceService>();

// Register Controllers with UserContextFilter
builder.Services.AddControllers(options =>
{
    options.Filters.Add<UserContextFilter>();
});

var app = builder.Build();

app.MapControllers();

app.Run();
