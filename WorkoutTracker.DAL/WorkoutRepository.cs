using WorkoutTracker.Models;
using MySqlConnector;

namespace WorkoutTracker.DAL;

public class WorkoutRepository
{
    private readonly DbConnectionFactory _factory;

    public WorkoutRepository(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<List<Workout>> GetAllAsync()
    {
        var workouts = new List<Workout>();

        using var conn = _factory.Create();
        await conn.OpenAsync();

        var cmd = new MySqlCommand("SELECT id, name, description FROM workout", conn);
        var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            workouts.Add(new Workout
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("name"),
                Description = reader.GetString("description")
            });
        }

        return workouts;
    }

    public async Task<Workout?> GetByIdAsync(int id)
    {
        using var conn = _factory.Create();
        await conn.OpenAsync();

        var cmd = new MySqlCommand(
            "SELECT id, name, description FROM workout WHERE id = @id", conn);

        cmd.Parameters.AddWithValue("@id", id);

        var reader = await cmd.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return new Workout
        {
            Id = reader.GetInt32("id"),
            Name = reader.GetString("name"),
            Description = reader.GetString("description")
        };
    }
}