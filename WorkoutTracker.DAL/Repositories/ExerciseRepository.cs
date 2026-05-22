using WorkoutTracker.Models;
using MySqlConnector;
using WorkoutTracker.Logic.Abstractions.Repositories;

namespace WorkoutTracker.DAL.Repositories;

/// Handles retrieval of personal and predefined exercises with their muscle group details.
public class ExerciseRepository(DbConnectionFactory connectionFactory) : IExerciseRepository
{
    /// Retrieves all exercises from the database that are either predefined or owned by the specified user.
    /// Joins with the muscle_groups table to populate category information.
    public async Task<List<Exercise>> GetAllAsync(int userId)
    {
        var exercises = new List<Exercise>();

        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var sqlQuery = @"
            SELECT exercise.id, exercise.user_id, exercise.name, exercise.description, exercise.type, exercise.primary_muscle_group_id, exercise.is_predefined, exercise.created_at,
                   muscleGroup.name as MuscleGroupName
            FROM exercises AS exercise
            LEFT JOIN muscle_groups AS muscleGroup ON exercise.primary_muscle_group_id = muscleGroup.id
            WHERE exercise.user_id = @userId OR exercise.is_predefined = TRUE 
            ORDER BY exercise.is_predefined DESC, exercise.name";

        var command = new MySqlCommand(sqlQuery, connection);
        command.Parameters.AddWithValue("@userId", userId);

        using var dataReader = await command.ExecuteReaderAsync();
        while (await dataReader.ReadAsync())
        {
            exercises.Add(MapExercise(dataReader));
        }

        return exercises;
    }

    /// Retrieves a single exercise by its identifier.
    public async Task<Exercise?> GetByIdAsync(int exerciseId)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var sqlQuery = @"
            SELECT exercise.id, exercise.user_id, exercise.name, exercise.description, exercise.type, exercise.primary_muscle_group_id, exercise.is_predefined, exercise.created_at,
                   muscleGroup.name as MuscleGroupName
            FROM exercises AS exercise
            LEFT JOIN muscle_groups AS muscleGroup ON exercise.primary_muscle_group_id = muscleGroup.id
            WHERE exercise.id = @id";

        var command = new MySqlCommand(sqlQuery, connection);
        command.Parameters.AddWithValue("@id", exerciseId);

        using var dataReader = await command.ExecuteReaderAsync();
        if (!await dataReader.ReadAsync())
            return null;

        return MapExercise(dataReader);
    }

    /// Inserts a new exercise record into the database.
    public async Task<int> CreateAsync(Exercise exercise)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var sqlQuery = @"
            INSERT INTO exercises (user_id, name, description, type, primary_muscle_group_id, is_predefined, created_at) 
            VALUES (@userId, @name, @description, @type, @muscleId, @isPredefined, @createdAt);
            SELECT LAST_INSERT_ID();";

        var command = new MySqlCommand(sqlQuery, connection);
        command.Parameters.AddWithValue("@userId", (object?)exercise.UserId ?? DBNull.Value);
        command.Parameters.AddWithValue("@name", exercise.Name);
        command.Parameters.AddWithValue("@description", (object?)exercise.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("@type", exercise.Type);
        command.Parameters.AddWithValue("@muscleId", (object?)exercise.PrimaryMuscleGroupId ?? DBNull.Value);
        command.Parameters.AddWithValue("@isPredefined", exercise.IsPredefined);
        command.Parameters.AddWithValue("@createdAt", exercise.CreatedAt);

        var resultId = await command.ExecuteScalarAsync();
        return Convert.ToInt32(resultId);
    }

    /// Updates an existing exercise record in the database.
    public async Task UpdateAsync(Exercise exercise)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var sqlQuery = @"
            UPDATE exercises 
            SET name = @name, 
                description = @description, 
                type = @type, 
                primary_muscle_group_id = @muscleId, 
                is_predefined = @isPredefined 
            WHERE id = @id";

        var command = new MySqlCommand(sqlQuery, connection);
        command.Parameters.AddWithValue("@name", exercise.Name);
        command.Parameters.AddWithValue("@description", (object?)exercise.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("@type", exercise.Type);
        command.Parameters.AddWithValue("@muscleId", (object?)exercise.PrimaryMuscleGroupId ?? DBNull.Value);
        command.Parameters.AddWithValue("@isPredefined", exercise.IsPredefined);
        command.Parameters.AddWithValue("@id", exercise.Id);


        await command.ExecuteNonQueryAsync();
    }

    /// Deletes an exercise record from the database.
    public async Task DeleteAsync(int exerciseId)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var command = new MySqlCommand("DELETE FROM exercises WHERE id = @id", connection);
        command.Parameters.AddWithValue("@id", exerciseId);

        await command.ExecuteNonQueryAsync();
    }

    /// Maps a database row from MySqlDataReader to an Exercise object.
    /// Handles optional muscle group name if present in the projection.
    private Exercise MapExercise(MySqlDataReader dataReader)
    {
        var exercise = new Exercise
        {
            Id = dataReader.GetInt32(dataReader.GetOrdinal("id")),
            UserId = dataReader.IsDBNull(dataReader.GetOrdinal("user_id")) ? null : dataReader.GetInt32(dataReader.GetOrdinal("user_id")),
            Name = dataReader.GetString(dataReader.GetOrdinal("name")),
            Description = dataReader.IsDBNull(dataReader.GetOrdinal("description")) ? null : dataReader.GetString(dataReader.GetOrdinal("description")),
            Type = dataReader.GetString(dataReader.GetOrdinal("type")),
            PrimaryMuscleGroupId = dataReader.IsDBNull(dataReader.GetOrdinal("primary_muscle_group_id")) ? null : dataReader.GetInt32(dataReader.GetOrdinal("primary_muscle_group_id")),
            IsPredefined = dataReader.GetBoolean(dataReader.GetOrdinal("is_predefined")),
            CreatedAt = dataReader.GetDateTime(dataReader.GetOrdinal("created_at"))
        };

        try 
        {
            int muscleGroupNameOrdinal = dataReader.GetOrdinal("MuscleGroupName");
            if (!dataReader.IsDBNull(muscleGroupNameOrdinal))
            {
                exercise.PrimaryMuscleGroup = new MuscleGroup
                {
                    Id = exercise.PrimaryMuscleGroupId ?? 0,
                    Name = dataReader.GetString(muscleGroupNameOrdinal)
                };
            }
        }
        catch (IndexOutOfRangeException)
        {
            // MuscleGroupName might not be in all queries
        }

        return exercise;
    }
}
