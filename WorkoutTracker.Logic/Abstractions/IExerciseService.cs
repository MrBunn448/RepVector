using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Services;

public interface IExerciseService
{
    Task<List<Exercise>> GetAllAsync(int userId);

    Task<Exercise?> GetByIdAsync(int exerciseId);

    Task<Result<int>> CreateAsync(Exercise exercise, User creator);

    Task<Result> UpdateAsync(Exercise exercise, User editor);

    Task<Result> DeleteAsync(int exerciseId, User deleter);
}
