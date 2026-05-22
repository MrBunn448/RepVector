using WorkoutTracker.UI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7162";

builder.Services.AddHttpClient<WorkoutApiClient>(client => client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<AuthApiClient>(client => client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<ExerciseApiClient>(client => client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<WorkoutExerciseApiClient>(client => client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<SessionApiClient>(client => client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<PreferenceApiClient>(client => client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<MetadataApiClient>(client => client.BaseAddress = new Uri(apiBaseUrl));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.MapGet("/", context =>
{
    context.Response.Redirect("/Workouts");
    return Task.CompletedTask;
});

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

app.Run();
