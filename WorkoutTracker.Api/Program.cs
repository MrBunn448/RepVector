using WorkoutTracker.DAL;
using WorkoutTracker.Logic;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddSingleton(new DbConnectionFactory(connectionString));

builder.Services.AddScoped<WorkoutRepository>();
builder.Services.AddScoped<WorkoutService>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();