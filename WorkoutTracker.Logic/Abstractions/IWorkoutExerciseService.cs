using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Services;

public interface IWorkoutExerciseService
{
    Task<Result<List<WorkoutExercise>>> GetByWorkoutIdAsync(int workoutId);

    Task<Result<WorkoutExercise>> GetByIdAsync(int workoutExerciseId);

    Task<Result> AddAsync(WorkoutExercise workoutExercise, User user);

    Task<Result> UpdateAsync(WorkoutExercise workoutExercise, User user);

    Task<Result> DeleteAsync(int workoutExerciseId, User user);
}
