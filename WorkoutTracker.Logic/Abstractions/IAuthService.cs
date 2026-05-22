using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Services;


/// Defines the contract for user authentication and registration services.
public interface IAuthService
{

    Task<Result<User>> RegisterAsync(string email, string password, string role = "User");

    Task<Result<User>> RegisterAdminAsync(string email, string password, string adminSecret);

    Task<Result<User>> LoginAsync(string email, string password);
}
