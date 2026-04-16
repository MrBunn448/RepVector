using RepVector.DAL;
using RepVector.DAL.Interfaces;
using RepVector.Logic;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IWorkoutRepository>(sp =>
    new WorkoutRepository(
        builder.Configuration.GetConnectionString("DefaultConnection")!
    ));

builder.Services.AddScoped<WorkoutService>();

var app = builder.Build();

app.MapControllers();
app.Run();