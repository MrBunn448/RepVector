using WorkoutTracker.DAL;
using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Services;

public class WorkoutService
{
    private readonly WorkoutRepository _workoutRepository;

    public WorkoutService(WorkoutRepository workoutRepository)
    {
        _workoutRepository = workoutRepository;
    }

    public async Task<List<Workout>> GetAllAsync()
    {
        return await _workoutRepository.GetAllAsync();
    }

    public async Task<Workout?> GetWorkoutDetailsAsync(int id)
    {
        // This one is currently sync in DAL, but we keep async at the boundary
        return await Task.FromResult(
            _workoutRepository.GetWorkoutWithExercises(id)
        );
    }
}