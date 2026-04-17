using WorkoutTracker.DAL;
using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Services;

public class WorkoutService
{
    private readonly IWorkoutRepository _workoutRepository;

    public WorkoutService(IWorkoutRepository workoutRepository)
    {
        _workoutRepository = workoutRepository;
    }

    public async Task<List<Workout>> GetAllAsync()
    {
        return await _workoutRepository.GetAllAsync();
    }

    public async Task<Workout?> GetWorkoutDetailsAsync(int id)
    {
        return await Task.FromResult(
            _workoutRepository.GetWorkoutWithExercises(id)
        );
    }
}
