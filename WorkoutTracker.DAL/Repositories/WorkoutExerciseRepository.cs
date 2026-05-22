using WorkoutTracker.Models;
using MySqlConnector;
using WorkoutTracker.Logic.Abstractions.Repositories;

namespace WorkoutTracker.DAL.Repositories;

/// Repository for linking exercises to workout templates.
/// Manages the many-to-many relationship between Workouts and Exercises using raw SQL.
public class WorkoutExerciseRepository(DbConnectionFactory connectionFactory) : IWorkoutExerciseRepository
{

    /// Retrieves all exercise links for a specific workout, including details of the linked exercise.
    public async Task<List<WorkoutExercise>> GetByWorkoutIdAsync(int workoutId)
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
            result.Add(MapWorkoutExercise(dataReader));
        }

        return result;
    }

    /// Retrieves a specific workout-exercise link by its ID.
    public async Task<WorkoutExercise?> GetByIdAsync(int id)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var sqlQuery = @"
            SELECT we.id, we.workout_id, we.exercise_id, we.target_sets, we.target_reps, we.target_rpe, we.sort_order,
                   e.name as ExerciseName, e.type as ExerciseType
            FROM workout_exercises we
            JOIN exercises e ON we.exercise_id = e.id
            WHERE we.id = @id";

        var command = new MySqlCommand(sqlQuery, connection);
        command.Parameters.AddWithValue("@id", id);

        using var dataReader = await command.ExecuteReaderAsync();
        if (!await dataReader.ReadAsync()) return null;

        return MapWorkoutExercise(dataReader);
    }

    /// Inserts a new link between a workout and an exercise.
    public async Task<int> CreateAsync(WorkoutExercise workoutExercise)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var sqlQuery = @"
            INSERT INTO workout_exercises (workout_id, exercise_id, target_sets, target_reps, target_rpe, sort_order) 
            VALUES (@workoutId, @exerciseId, @targetSets, @targetReps, @targetRpe, @sortOrder);
            SELECT LAST_INSERT_ID();";

        var command = new MySqlCommand(sqlQuery, connection);
        command.Parameters.AddWithValue("@workoutId", workoutExercise.WorkoutId);
        command.Parameters.AddWithValue("@exerciseId", workoutExercise.ExerciseId);
        command.Parameters.AddWithValue("@targetSets", workoutExercise.TargetSets);
        command.Parameters.AddWithValue("@targetReps", workoutExercise.TargetReps);
        command.Parameters.AddWithValue("@targetRpe", (object?)workoutExercise.TargetRpe ?? DBNull.Value);
        command.Parameters.AddWithValue("@sortOrder", workoutExercise.SortOrder);

        var resultId = await command.ExecuteScalarAsync();
        return Convert.ToInt32(resultId);
    }

    /// Updates an existing link between a workout and an exercise.
    public async Task UpdateAsync(WorkoutExercise workoutExercise)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var sqlQuery = @"
            UPDATE workout_exercises 
            SET exercise_id = @exerciseId, 
                target_sets = @targetSets, 
                target_reps = @targetReps, 
                target_rpe = @targetRpe, 
                sort_order = @sortOrder 
            WHERE id = @id";

        var command = new MySqlCommand(sqlQuery, connection);
        command.Parameters.AddWithValue("@exerciseId", workoutExercise.ExerciseId);
        command.Parameters.AddWithValue("@targetSets", workoutExercise.TargetSets);
        command.Parameters.AddWithValue("@targetReps", workoutExercise.TargetReps);
        command.Parameters.AddWithValue("@targetRpe", (object?)workoutExercise.TargetRpe ?? DBNull.Value);
        command.Parameters.AddWithValue("@sortOrder", workoutExercise.SortOrder);
        command.Parameters.AddWithValue("@id", workoutExercise.Id);

        await command.ExecuteNonQueryAsync();
    }

    /// Removes a link between a workout and an exercise.
    public async Task DeleteAsync(int id)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var command = new MySqlCommand("DELETE FROM workout_exercises WHERE id = @id", connection);
        command.Parameters.AddWithValue("@id", id);

        await command.ExecuteNonQueryAsync();
    }

    /// Maps a database row from MySqlDataReader to a WorkoutExercise object.
    /// Populates a nested Exercise object with basic metadata.
    private WorkoutExercise MapWorkoutExercise(MySqlDataReader dataReader)
    {
        return new WorkoutExercise
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
        };
    }
}
