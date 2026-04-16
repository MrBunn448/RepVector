using MySqlConnector;
using RepVector.DAL.Interfaces;
using RepVector.Models;

namespace RepVector.DAL;

public class WorkoutRepository : IWorkoutRepository
{
    private readonly string _connectionString;

    public WorkoutRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IEnumerable<Workout> GetAll()
    {
        var workouts = new List<Workout>();

        using var connection = new MySqlConnection(_connectionString);
        connection.Open();

        var cmd = new MySqlCommand("SELECT id, name, description FROM workout", connection);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
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

    public Workout? GetById(int id)
    {
        using var connection = new MySqlConnection(_connectionString);
        connection.Open();

        var workoutCmd = new MySqlCommand(
            "SELECT id, name, description FROM workout WHERE id = @id",
            connection);
        workoutCmd.Parameters.AddWithValue("@id", id);

        Workout? workout = null;

        using (var reader = workoutCmd.ExecuteReader())
        {
            if (reader.Read())
            {
                workout = new Workout
                {
                    Id = reader.GetInt32("id"),
                    Name = reader.GetString("name"),
                    Description = reader.GetString("description")
                };
            }
        }

        if (workout == null)
            return null;

        var exerciseCmd = new MySqlCommand(
            "SELECT id, name, reps, sets FROM exercise WHERE workout_id = @id",
            connection);
        exerciseCmd.Parameters.AddWithValue("@id", id);

        using var exerciseReader = exerciseCmd.ExecuteReader();
        while (exerciseReader.Read())
        {
            workout.Exercises.Add(new Exercise
            {
                Id = exerciseReader.GetInt32("id"),
                Name = exerciseReader.GetString("name"),
                Reps = exerciseReader.GetInt32("reps"),
                Sets = exerciseReader.GetInt32("sets"),
                WorkoutId = id
            });
        }

        return workout;
    }
}