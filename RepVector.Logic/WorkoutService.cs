using RepVector.DAL.Interfaces;
using RepVector.Models;

namespace RepVector.Logic;

public class WorkoutService
{
    private readonly IWorkoutRepository _repository;

    public WorkoutService(IWorkoutRepository repository)
    {
        _repository = repository;
    }

    public IEnumerable<Workout> GetWorkouts()
    {
        return _repository.GetAll();
    }

    public Workout? GetWorkout(int id)
    {
        return _repository.GetById(id);
    }
}