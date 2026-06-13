using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Abstractions.Services;

public interface IMetadataService
{
    Task<Result<List<MuscleGroup>>> GetAllMuscleGroupsAsync();
    Task<Result<int>> CreateMuscleGroupAsync(MuscleGroup muscleGroup);
    Task<Result> DeleteMuscleGroupAsync(int id);
}
