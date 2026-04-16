using WorkoutTracker.DAL;
using WorkoutTracker.Models;

namespace WorkoutTracker.Logic;

public class WorkoutService
{
    private readonly WorkoutRepository _repo;

    public WorkoutService(WorkoutRepository repo)
    {
        _repo = repo;
    }

    public Task<List<Workout>> GetAllWorkouts()
        => _repo.GetAllAsync();

    public Task<Workout?> GetWorkout(int id)
        => _repo.GetByIdAsync(id);
}