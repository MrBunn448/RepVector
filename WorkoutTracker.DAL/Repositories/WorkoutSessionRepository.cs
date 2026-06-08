using WorkoutTracker.Models;
using MySqlConnector;
using WorkoutTracker.Logic.Abstractions.Repositories;

namespace WorkoutTracker.DAL.Repositories;

/// Repository for managing live workout sessions and set logging.
/// Handles persistence of session metadata and individual performance sets using raw SQL.

public class WorkoutSessionRepository(DbConnectionFactory connectionFactory) : IWorkoutSessionRepository
{

    /// Retrieves a specific workout session by its identifier.
    public async Task<WorkoutSession?> GetByIdAsync(int id)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var command = new MySqlCommand("SELECT id, user_id, workout_id, workout_name, started_at, finished_at, status, total_seconds FROM workout_sessions WHERE id = @id", connection);
        command.Parameters.AddWithValue("@id", id);

        using var dataReader = await command.ExecuteReaderAsync();
        if (!await dataReader.ReadAsync()) return null;

        return MapSession(dataReader);
    }

    /// Retrieves all historical sessions belonging to a specific user.
    public async Task<List<WorkoutSession>> GetByUserIdAsync(int userId)
    {
        var result = new List<WorkoutSession>();
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var command = new MySqlCommand("SELECT id, user_id, workout_id, workout_name, started_at, finished_at, status, total_seconds FROM workout_sessions WHERE user_id = @userId ORDER BY started_at DESC", connection);
        command.Parameters.AddWithValue("@userId", userId);

        using var dataReader = await command.ExecuteReaderAsync();
        while (await dataReader.ReadAsync())
        {
            result.Add(MapSession(dataReader));
        }

        return result;
    }

    /// Finds the currently active (ongoing) session for a user.
    public async Task<WorkoutSession?> GetActiveSessionByUserIdAsync(int userId)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var command = new MySqlCommand("SELECT id, user_id, workout_id, workout_name, started_at, finished_at, status, total_seconds FROM workout_sessions WHERE user_id = @userId AND status = 'active' LIMIT 1", connection);
        command.Parameters.AddWithValue("@userId", userId);

        using var dataReader = await command.ExecuteReaderAsync();
        if (!await dataReader.ReadAsync()) return null;

        return MapSession(dataReader);
    }

    /// Inserts a new workout session record into the database.
    public async Task<int> CreateAsync(WorkoutSession session)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var sqlQuery = @"
            INSERT INTO workout_sessions (user_id, workout_id, workout_name, started_at, finished_at, status, total_seconds) 
            VALUES (@userId, @workoutId, @workoutName, @startedAt, @finishedAt, @status, @totalSeconds);
            SELECT LAST_INSERT_ID();";

        var command = new MySqlCommand(sqlQuery, connection);
        command.Parameters.AddWithValue("@userId", session.UserId);
        command.Parameters.AddWithValue("@workoutId", (object?)session.WorkoutId ?? DBNull.Value);
        command.Parameters.AddWithValue("@workoutName", (object?)session.WorkoutName ?? DBNull.Value);
        command.Parameters.AddWithValue("@startedAt", session.StartedAt);
        command.Parameters.AddWithValue("@finishedAt", (object?)session.FinishedAt ?? DBNull.Value);
        command.Parameters.AddWithValue("@status", session.Status);
        command.Parameters.AddWithValue("@totalSeconds", session.TotalSeconds);

        var resultId = await command.ExecuteScalarAsync();
        return Convert.ToInt32(resultId);
    }

    /// Updates an existing workout session record (e.g., when finishing or cancelling).
    public async Task UpdateAsync(WorkoutSession session)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var sqlQuery = @"
            UPDATE workout_sessions 
            SET finished_at = @finishedAt, 
                status = @status, 
                total_seconds = @totalSeconds 
            WHERE id = @id";

        var command = new MySqlCommand(sqlQuery, connection);
        command.Parameters.AddWithValue("@finishedAt", (object?)session.FinishedAt ?? DBNull.Value);
        command.Parameters.AddWithValue("@status", session.Status);
        command.Parameters.AddWithValue("@totalSeconds", session.TotalSeconds);
        command.Parameters.AddWithValue("@id", session.Id);

        await command.ExecuteNonQueryAsync();
    }

    /// Deletes a workout session and all its associated set logs.
    public async Task DeleteAsync(int id)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var command = new MySqlCommand("DELETE FROM workout_sessions WHERE id = @id", connection);
        command.Parameters.AddWithValue("@id", id);

        await command.ExecuteNonQueryAsync();
    }

    /// Records a performance set log (weight/reps) for a session.
    public async Task<int> AddSetLogAsync(WorkoutSetLog log)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var sqlQuery = @"
            INSERT INTO workout_set_logs (session_id, exercise_id, set_number, weight, reps, rpe, set_type, completed_at) 
            VALUES (@sessionId, @exerciseId, @setNumber, @weight, @reps, @rpe, @setType, @completedAt);
            SELECT LAST_INSERT_ID();";

        var command = new MySqlCommand(sqlQuery, connection);
        command.Parameters.AddWithValue("@sessionId", log.SessionId);
        command.Parameters.AddWithValue("@exerciseId", log.ExerciseId);
        command.Parameters.AddWithValue("@setNumber", log.SetNumber);
        command.Parameters.AddWithValue("@weight", log.Weight);
        command.Parameters.AddWithValue("@reps", log.Reps);
        command.Parameters.AddWithValue("@rpe", (object?)log.Rpe ?? DBNull.Value);
        command.Parameters.AddWithValue("@setType", log.SetType);
        command.Parameters.AddWithValue("@completedAt", log.CompletedAt);

        var resultId = await command.ExecuteScalarAsync();
        return Convert.ToInt32(resultId);
    }

    /// Retrieves all sets logged during a specific workout session.
    public async Task<List<WorkoutSetLog>> GetSetLogsBySessionIdAsync(int sessionId)
    {
        var result = new List<WorkoutSetLog>();
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var sqlQuery = "SELECT id, session_id, exercise_id, set_number, weight, reps, rpe, set_type, completed_at FROM workout_set_logs WHERE session_id = @sessionId ORDER BY completed_at";
        var command = new MySqlCommand(sqlQuery, connection);
        command.Parameters.AddWithValue("@sessionId", sessionId);

        using var dataReader = await command.ExecuteReaderAsync();
        while (await dataReader.ReadAsync())
        {
            result.Add(new WorkoutSetLog
            {
                Id = dataReader.GetInt32(dataReader.GetOrdinal("id")),
                SessionId = dataReader.GetInt32(dataReader.GetOrdinal("session_id")),
                ExerciseId = dataReader.GetInt32(dataReader.GetOrdinal("exercise_id")),
                SetNumber = dataReader.GetInt32(dataReader.GetOrdinal("set_number")),
                Weight = dataReader.GetDecimal(dataReader.GetOrdinal("weight")),
                Reps = dataReader.GetInt32(dataReader.GetOrdinal("reps")),
                Rpe = dataReader.IsDBNull(dataReader.GetOrdinal("rpe")) ? null : dataReader.GetInt32(dataReader.GetOrdinal("rpe")),
                SetType = dataReader.GetString(dataReader.GetOrdinal("set_type")),
                CompletedAt = dataReader.GetDateTime(dataReader.GetOrdinal("completed_at"))
            });
        }

        return result;
    }

    /// Deletes a specific set log by its ID.
    public async Task DeleteSetLogAsync(int logId)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var command = new MySqlCommand("DELETE FROM workout_set_logs WHERE id = @id", connection);
        command.Parameters.AddWithValue("@id", logId);

        await command.ExecuteNonQueryAsync();
    }

    /// Deletes all set logs for a specific exercise in a session.
    public async Task DeleteExerciseLogsAsync(int sessionId, int exerciseId)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var command = new MySqlCommand("DELETE FROM workout_set_logs WHERE session_id = @sessionId AND exercise_id = @exerciseId", connection);
        command.Parameters.AddWithValue("@sessionId", sessionId);
        command.Parameters.AddWithValue("@exerciseId", exerciseId);

        await command.ExecuteNonQueryAsync();
    }

    /// Maps a database row from MySqlDataReader to a WorkoutSession object.
    private WorkoutSession MapSession(MySqlDataReader dataReader)
    {
        return new WorkoutSession
        {
            Id = dataReader.GetInt32(dataReader.GetOrdinal("id")),
            UserId = dataReader.GetInt32(dataReader.GetOrdinal("user_id")),
            WorkoutId = dataReader.IsDBNull(dataReader.GetOrdinal("workout_id")) ? null : dataReader.GetInt32(dataReader.GetOrdinal("workout_id")),
            WorkoutName = dataReader.IsDBNull(dataReader.GetOrdinal("workout_name")) ? null : dataReader.GetString(dataReader.GetOrdinal("workout_name")),
            StartedAt = dataReader.GetDateTime(dataReader.GetOrdinal("started_at")),
            FinishedAt = dataReader.IsDBNull(dataReader.GetOrdinal("finished_at")) ? null : dataReader.GetDateTime(dataReader.GetOrdinal("finished_at")),
            Status = dataReader.GetString(dataReader.GetOrdinal("status")),
            TotalSeconds = dataReader.GetInt32(dataReader.GetOrdinal("total_seconds"))
        };
    }
}
