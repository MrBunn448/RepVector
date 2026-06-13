using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Services;

public interface IWorkoutService
{
    Task<Result<List<Workout>>> GetAllByUserIdAsync(int userId);

    Task<Result<Workout>> GetByIdAsync(int workoutId);

    Task<Result<Workout>> GetWorkoutDetailsAsync(int workoutId);

    Task<Result<int>> CreateWorkoutAsync(Workout workout, User creator);

    Task<Result> UpdateWorkoutAsync(Workout workout, User editor);

    Task<Result> DeleteWorkoutAsync(int workoutId, User deleter);
}
