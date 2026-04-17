using WorkoutTracker.Models;
using MySqlConnector;
using System.Data;

namespace WorkoutTracker.DAL;

public class WorkoutRepository : IWorkoutRepository
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

    public Workout? GetWorkoutWithExercises(int workoutId)
    {
        Workout? workout = null;

        using var connection = _factory.Create();
        connection.Open();

        var sql = @"
        SELECT 
            w.id AS WorkoutId,
            w.name AS WorkoutName,
            w.description,
            e.id AS ExerciseId,
            e.name AS ExerciseName,
            e.sets,
            e.reps
        FROM workout w
        LEFT JOIN exercise e ON e.workout_id = w.id
        WHERE w.id = @id;
    ";

        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", workoutId);

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            if (workout == null)
            {
                workout = new Workout
                {
                    Id = reader.GetInt32("WorkoutId"),
                    Name = reader.GetString("WorkoutName"),
                    Description = reader.GetString("description")
                };
            }

            if (!reader.IsDBNull("ExerciseId"))
            {
                workout.Exercises.Add(new Exercise
                {
                    Id = reader.GetInt32("ExerciseId"),
                    Name = reader.GetString("ExerciseName"),
                    Sets = reader.GetInt32("sets"),
                    Reps = reader.GetInt32("reps"),
                    WorkoutId = workout.Id
                });
            }
        }

        return workout;
    }
}