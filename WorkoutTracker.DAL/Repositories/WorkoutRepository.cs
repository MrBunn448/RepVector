using WorkoutTracker.Models;
using MySqlConnector;
using WorkoutTracker.Logic.Abstractions.Repositories;

namespace WorkoutTracker.DAL.Repositories;

/// Repository for managing workout templates.
/// Handles persistence of workout metadata and complex retrieval of workouts with nested exercises using raw SQL.
public class WorkoutRepository(DbConnectionFactory connectionFactory) : IWorkoutRepository
{
    /// Retrieves all workout templates accessible to a user (their own + predefined ones).
    public async Task<List<Workout>> GetAllByUserIdAsync(int userId)
    {
        var result = new List<Workout>();
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var sqlQuery = "SELECT id, user_id, name, description, is_predefined, created_at FROM workouts WHERE user_id = @userId OR is_predefined = TRUE ORDER BY is_predefined DESC, name";
        var command = new MySqlCommand(sqlQuery, connection);
        command.Parameters.AddWithValue("@userId", userId);

        using var dataReader = await command.ExecuteReaderAsync();
        while (await dataReader.ReadAsync())
        {
            result.Add(MapWorkout(dataReader));
        }

        return result;
    }

    /// Retrieves a single workout template by its identifier.
    public async Task<Workout?> GetByIdAsync(int id)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var command = new MySqlCommand("SELECT id, user_id, name, description, is_predefined, created_at FROM workouts WHERE id = @id", connection);
        command.Parameters.AddWithValue("@id", id);

        using var dataReader = await command.ExecuteReaderAsync();
        if (!await dataReader.ReadAsync()) return null;

        return MapWorkout(dataReader);
    }

    /// Retrieves a workout template along with all its associated exercises.
    /// Performs two separate database calls to build the complex object.
    public async Task<Workout?> GetWorkoutWithExercisesAsync(int id)
    {
        var workout = await GetByIdAsync(id);
        if (workout == null) return null;

        workout.Exercises = await GetExercisesByWorkoutIdAsync(id);
        return workout;
    }

    /// Inserts a new workout template into the database.
    public async Task<int> CreateAsync(Workout workout)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var sqlQuery = @"
            INSERT INTO workouts (user_id, name, description, is_predefined, created_at) 
            VALUES (@userId, @name, @description, @isPredefined, @createdAt);
            SELECT LAST_INSERT_ID();";

        var command = new MySqlCommand(sqlQuery, connection);
        command.Parameters.AddWithValue("@userId", (object?)workout.UserId ?? DBNull.Value);
        command.Parameters.AddWithValue("@name", workout.Name);
        command.Parameters.AddWithValue("@description", (object?)workout.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("@isPredefined", workout.IsPredefined);
        command.Parameters.AddWithValue("@createdAt", workout.CreatedAt);

        var resultId = await command.ExecuteScalarAsync();
        return Convert.ToInt32(resultId);
    }

    /// Updates an existing workout template record.
    public async Task UpdateAsync(Workout workout)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var sqlQuery = @"
            UPDATE workouts 
            SET name = @name, 
                description = @description, 
                is_predefined = @isPredefined 
            WHERE id = @id";

        var command = new MySqlCommand(sqlQuery, connection);
        command.Parameters.AddWithValue("@name", workout.Name);
        command.Parameters.AddWithValue("@description", (object?)workout.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("@isPredefined", workout.IsPredefined);
        command.Parameters.AddWithValue("@id", workout.Id);

        await command.ExecuteNonQueryAsync();
    }

    /// Deletes a workout template from the database.
    public async Task DeleteAsync(int id)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var command = new MySqlCommand("DELETE FROM workouts WHERE id = @id", connection);
        command.Parameters.AddWithValue("@id", id);

        await command.ExecuteNonQueryAsync();
    }

    /// Internal helper to retrieve the list of exercises associated with a workout.
    private async Task<List<WorkoutExercise>> GetExercisesByWorkoutIdAsync(int workoutId)
    {
        var result = new List<WorkoutExercise>();
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var sqlQuery = @"
            SELECT we.id, we.workout_id, we.exercise_id, we.target_sets, we.target_reps, we.target_rpe, we.sort_order,
                   e.name as ExerciseName, e.type as ExerciseType
            FROM workout_exercises we
            JOIN exercises e ON we.exercise_id = e.id
            WHERE we.workout_id = @workoutId
            ORDER BY we.sort_order";

        var command = new MySqlCommand(sqlQuery, connection);
        command.Parameters.AddWithValue("@workoutId", workoutId);

        using var dataReader = await command.ExecuteReaderAsync();
        while (await dataReader.ReadAsync())
        {
            result.Add(new WorkoutExercise
            {
                Id = dataReader.GetInt32(dataReader.GetOrdinal("id")),
                WorkoutId = dataReader.GetInt32(dataReader.GetOrdinal("workout_id")),
                ExerciseId = dataReader.GetInt32(dataReader.GetOrdinal("exercise_id")),
                TargetSets = dataReader.GetInt32(dataReader.GetOrdinal("target_sets")),
                TargetReps = dataReader.GetInt32(dataReader.GetOrdinal("target_reps")),
                TargetRpe = dataReader.IsDBNull(dataReader.GetOrdinal("target_rpe")) ? null : dataReader.GetInt32(dataReader.GetOrdinal("target_rpe")),
                SortOrder = dataReader.GetInt32(dataReader.GetOrdinal("sort_order")),
                Exercise = new Exercise
                {
                    Id = dataReader.GetInt32(dataReader.GetOrdinal("exercise_id")),
                    Name = dataReader.GetString(dataReader.GetOrdinal("ExerciseName")),
                    Type = dataReader.GetString(dataReader.GetOrdinal("ExerciseType"))
                }
            });
        }

        return result;
    }

    /// Maps a database row from MySqlDataReader to a Workout object.
    private Workout MapWorkout(MySqlDataReader dataReader)
    {
        return new Workout
        {
            Id = dataReader.GetInt32(dataReader.GetOrdinal("id")),
            UserId = dataReader.IsDBNull(dataReader.GetOrdinal("user_id")) ? null : dataReader.GetInt32(dataReader.GetOrdinal("user_id")),
            Name = dataReader.GetString(dataReader.GetOrdinal("name")),
            Description = dataReader.IsDBNull(dataReader.GetOrdinal("description")) ? null : dataReader.GetString(dataReader.GetOrdinal("description")),
            IsPredefined = dataReader.GetBoolean(dataReader.GetOrdinal("is_predefined")),
            CreatedAt = dataReader.GetDateTime(dataReader.GetOrdinal("created_at"))
        };
    }
}
