using WorkoutTracker.Models;
using MySqlConnector;
using WorkoutTracker.Logic.Abstractions.Repositories;

namespace WorkoutTracker.DAL.Repositories;

/// Repository for user account data.
/// Handles authentication and user management operations using raw SQL.
public class UserRepository(DbConnectionFactory connectionFactory) : IUserRepository
{
    /// Retrieves a user by their unique identifier.
    public async Task<User?> GetByIdAsync(int id)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var command = new MySqlCommand("SELECT id, email, password_hash, role, created_at FROM users WHERE id = @id", connection);
        command.Parameters.AddWithValue("@id", id);

        using var dataReader = await command.ExecuteReaderAsync();
        if (!await dataReader.ReadAsync()) return null;

        return MapUser(dataReader);
    }

    /// Retrieves a user by their email address. Primary method for finding users during login.
    public async Task<User?> GetByEmailAsync(string email)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var command = new MySqlCommand("SELECT id, email, password_hash, role, created_at FROM users WHERE email = @email", connection);
        command.Parameters.AddWithValue("@email", email);

        using var dataReader = await command.ExecuteReaderAsync();
        if (!await dataReader.ReadAsync()) return null;

        return MapUser(dataReader);
    }

    /// Inserts a new user record into the database.
    public async Task<int> CreateAsync(User user)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var command = new MySqlCommand(
            "INSERT INTO users (email, password_hash, role, created_at) VALUES (@email, @passwordHash, @role, @createdAt); SELECT LAST_INSERT_ID();",
            connection);
        command.Parameters.AddWithValue("@email", user.Email);
        command.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
        command.Parameters.AddWithValue("@role", user.Role);
        command.Parameters.AddWithValue("@createdAt", user.CreatedAt);

        var resultId = await command.ExecuteScalarAsync();
        return Convert.ToInt32(resultId);
    }

    /// Updates an existing user record.
    public async Task UpdateAsync(User user)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var command = new MySqlCommand(
            "UPDATE users SET email = @email, password_hash = @passwordHash, role = @role WHERE id = @id",
            connection);
        command.Parameters.AddWithValue("@email", user.Email);
        command.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
        command.Parameters.AddWithValue("@role", user.Role);
        command.Parameters.AddWithValue("@id", user.Id);

        await command.ExecuteNonQueryAsync();
    }

    /// Deletes a user account from the database.
    public async Task DeleteAsync(int id)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var command = new MySqlCommand("DELETE FROM users WHERE id = @id", connection);
        command.Parameters.AddWithValue("@id", id);

        await command.ExecuteNonQueryAsync();
    }

    /// Maps a database row from MySqlDataReader to a User object.
    private User MapUser(MySqlDataReader dataReader)
    {
        return new User
        {
            Id = dataReader.GetInt32(dataReader.GetOrdinal("id")),
            Email = dataReader.GetString(dataReader.GetOrdinal("email")),
            PasswordHash = dataReader.GetString(dataReader.GetOrdinal("password_hash")),
            Role = dataReader.GetString(dataReader.GetOrdinal("role")),
            CreatedAt = dataReader.GetDateTime(dataReader.GetOrdinal("created_at"))
        };
    }
}
