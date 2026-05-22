using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using WorkoutTracker.Logic.Abstractions.Repositories;
using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Services;

/// Service for user authentication and account management.
/// Uses PBKDF2 for secure password hashing and the Result pattern for status reporting.
public class AuthService(IUserRepository userRepository, IConfiguration configuration) : IAuthService
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 10000;

    /// Registers a new user with a hashed password.
    public async Task<Result<User>> RegisterAsync(string email, string password, string role = "User")
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return Result<User>.Failure("Email and password are required.");

        if (password.Length < 6)
            return Result<User>.Failure("Password must be at least 6 characters long.");

        var existingUser = await userRepository.GetByEmailAsync(email);
        if (existingUser != null)
            return Result<User>.Failure("User with this email already exists.", ResultType.Conflict);

        var passwordHash = HashPassword(password);
        var user = new User
        {
            Email = email,
            PasswordHash = passwordHash,
            Role = role,
            CreatedAt = DateTime.UtcNow
        };

        var userId = await userRepository.CreateAsync(user);
        user.Id = userId;

        return Result<User>.Success(user);
    }

    /// Registers a new administrative user if the provided secret matches the system configuration.
    public async Task<Result<User>> RegisterAdminAsync(string email, string password, string adminSecret)
    {
        var requiredSecret = configuration["AdminSecretKey"];

        if (string.IsNullOrEmpty(requiredSecret))
            return Result<User>.Failure("Admin registration is currently disabled (Secret Key not configured).", ResultType.Forbidden);

        if (adminSecret != requiredSecret)
            return Result<User>.Forbidden("Invalid admin secret.");

        return await RegisterAsync(email, password, "Admin");
    }

    /// Authenticates a user by verifying the provided password against the stored hash.
    public async Task<Result<User>> LoginAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return Result<User>.Failure("Email and password are required.");

        var user = await userRepository.GetByEmailAsync(email);
        if (user == null)
            return Result<User>.Failure("Invalid email or password.");

        if (!VerifyPassword(password, user.PasswordHash))
            return Result<User>.Failure("Invalid email or password.");

        return Result<User>.Success(user);
    }

    /// Generates a secure hash for a password using a random salt and PBKDF2.
    private string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);

        var hashBytes = new byte[SaltSize + HashSize];
        Array.Copy(salt, 0, hashBytes, 0, SaltSize);
        Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

        return Convert.ToBase64String(hashBytes);
    }

    /// Verifies a plain-text password against a stored base64 hash.
    private bool VerifyPassword(string password, string hash)
    {
        var hashBytes = Convert.FromBase64String(hash);

        if (hashBytes.Length != SaltSize + HashSize)
            return false;

        var salt = new byte[SaltSize];
        Array.Copy(hashBytes, 0, salt, 0, SaltSize);

        var expectedHash = new byte[HashSize];
        Array.Copy(hashBytes, SaltSize, expectedHash, 0, HashSize);

        var actualHash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);

        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
}
