using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Services;

public interface IWorkoutService
{
    Task<List<Workout>> GetAllByUserIdAsync(int userId);

    Task<Workout?> GetByIdAsync(int workoutId);

    Task<Workout?> GetWorkoutDetailsAsync(int workoutId);

    Task<Result<int>> CreateWorkoutAsync(Workout workout, User creator);

    Task<Result> UpdateWorkoutAsync(Workout workout, User editor);

    Task<Result> DeleteWorkoutAsync(int workoutId, User deleter);
}
