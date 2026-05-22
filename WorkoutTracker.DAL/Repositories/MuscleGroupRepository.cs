using WorkoutTracker.Models;
using MySqlConnector;
using WorkoutTracker.Logic.Abstractions.Repositories;

namespace WorkoutTracker.DAL.Repositories;

/// Repository for accessing muscle group metadata.

public class MuscleGroupRepository(DbConnectionFactory connectionFactory) : IMuscleGroupRepository
{
    /// Retrieves all muscle groups from the database.
    public async Task<List<MuscleGroup>> GetAllAsync()
    {
        var result = new List<MuscleGroup>();
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var command = new MySqlCommand("SELECT id, name FROM muscle_groups ORDER BY name", connection);
        using var dataReader = await command.ExecuteReaderAsync();
        while (await dataReader.ReadAsync())
        {
            result.Add(new MuscleGroup
            {
                Id = dataReader.GetInt32(dataReader.GetOrdinal("id")),
                Name = dataReader.GetString(dataReader.GetOrdinal("name"))
            });
        }

        return result;
    }

    /// Retrieves a specific muscle group by its identifier.
    public async Task<MuscleGroup?> GetByIdAsync(int muscleGroupId)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var command = new MySqlCommand("SELECT id, name FROM muscle_groups WHERE id = @id", connection);
        command.Parameters.AddWithValue("@id", muscleGroupId);

        using var dataReader = await command.ExecuteReaderAsync();
        if (!await dataReader.ReadAsync()) return null;

        return new MuscleGroup
        {
            Id = dataReader.GetInt32(dataReader.GetOrdinal("id")),
            Name = dataReader.GetString(dataReader.GetOrdinal("name"))
        };
    }

    /// Inserts a new muscle group into the database.
    public async Task<int> CreateAsync(MuscleGroup muscleGroup)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var command = new MySqlCommand("INSERT INTO muscle_groups (name) VALUES (@name); SELECT LAST_INSERT_ID();", connection);
        command.Parameters.AddWithValue("@name", muscleGroup.Name);

        var resultId = await command.ExecuteScalarAsync();
        return Convert.ToInt32(resultId);
    }

    /// Updates an existing muscle group record.
    public async Task UpdateAsync(MuscleGroup muscleGroup)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var command = new MySqlCommand("UPDATE muscle_groups SET name = @name WHERE id = @id", connection);
        command.Parameters.AddWithValue("@name", muscleGroup.Name);
        command.Parameters.AddWithValue("@id", muscleGroup.Id);

        await command.ExecuteNonQueryAsync();
    }

    /// Removes a muscle group from the database.
    public async Task DeleteAsync(int muscleGroupId)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var command = new MySqlCommand("DELETE FROM muscle_groups WHERE id = @id", connection);
        command.Parameters.AddWithValue("@id", muscleGroupId);

        await command.ExecuteNonQueryAsync();
    }
}
