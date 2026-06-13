using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Services;

public interface IExerciseService
{
    Task<Result<List<Exercise>>> GetAllAsync(int userId);

    Task<Result<Exercise>> GetByIdAsync(int exerciseId);

    Task<Result<int>> CreateAsync(Exercise exercise, User creator);

    Task<Result> UpdateAsync(Exercise exercise, User editor);

    Task<Result> DeleteAsync(int exerciseId, User deleter);
}
