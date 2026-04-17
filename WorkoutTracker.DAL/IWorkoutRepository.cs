using WorkoutTracker.Models;

namespace WorkoutTracker.DAL;

public interface IWorkoutRepository
{
    Task<List<Workout>> GetAllAsync();
    Task<Workout?> GetByIdAsync(int id);
    Workout? GetWorkoutWithExercises(int workoutId);
}
