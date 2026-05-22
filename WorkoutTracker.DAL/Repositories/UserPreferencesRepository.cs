using WorkoutTracker.Models;
using MySqlConnector;
using WorkoutTracker.Logic.Abstractions.Repositories;

namespace WorkoutTracker.DAL.Repositories;

/// Repository for managing user-specific settings.
/// Handles persistence of measurement units and display names using raw SQL.

public class UserPreferencesRepository(DbConnectionFactory connectionFactory) : IUserPreferencesRepository
{
    /// Retrieves the preferences for a specific user from the database.
    public async Task<UserPreferences?> GetByUserIdAsync(int userId)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var command = new MySqlCommand("SELECT user_id, username, weight_unit, distance_unit FROM user_preferences WHERE user_id = @userId", connection);
        command.Parameters.AddWithValue("@userId", userId);

        using var dataReader = await command.ExecuteReaderAsync();
        if (!await dataReader.ReadAsync()) return null;

        return new UserPreferences
        {
            UserId = dataReader.GetInt32(dataReader.GetOrdinal("user_id")),
            Username = dataReader.IsDBNull(dataReader.GetOrdinal("username")) ? null : dataReader.GetString(dataReader.GetOrdinal("username")),
            WeightUnit = dataReader.GetString(dataReader.GetOrdinal("weight_unit")),
            DistanceUnit = dataReader.GetString(dataReader.GetOrdinal("distance_unit"))
        };
    }

    /// Saves user preferences to the database. Uses an UPSERT (ON DUPLICATE KEY UPDATE) logic.
    public async Task CreateOrUpdateAsync(UserPreferences userPreferences)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var sqlQuery = @"
            INSERT INTO user_preferences (user_id, username, weight_unit, distance_unit)
            VALUES (@userId, @username, @weightUnit, @distanceUnit)
            ON DUPLICATE KEY UPDATE 
                username = @username, 
                weight_unit = @weightUnit, 
                distance_unit = @distanceUnit";

        var command = new MySqlCommand(sqlQuery, connection);
        command.Parameters.AddWithValue("@userId", userPreferences.UserId);
        command.Parameters.AddWithValue("@username", (object?)userPreferences.Username ?? DBNull.Value);
        command.Parameters.AddWithValue("@weightUnit", userPreferences.WeightUnit);
        command.Parameters.AddWithValue("@distanceUnit", userPreferences.DistanceUnit);

        await command.ExecuteNonQueryAsync();
    }
}
