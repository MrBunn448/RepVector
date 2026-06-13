using WorkoutTracker.Logic.Abstractions.Repositories;
using WorkoutTracker.Logic.Abstractions.Services;
using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Services;

public class MetadataService(IMuscleGroupRepository muscleGroupRepository) : IMetadataService
{
    public async Task<Result<List<MuscleGroup>>> GetAllMuscleGroupsAsync()
    {
        try
        {
            var groups = await muscleGroupRepository.GetAllAsync();
            return Result<List<MuscleGroup>>.Success(groups);
        }
        catch (Exception ex)
        {
            return Result<List<MuscleGroup>>.Failure($"Failed to retrieve muscle groups: {ex.Message}", ResultType.Error);
        }
    }

    public async Task<Result<int>> CreateMuscleGroupAsync(MuscleGroup muscleGroup)
    {
        try
        {
            var id = await muscleGroupRepository.CreateAsync(muscleGroup);
            return Result<int>.Success(id);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure($"Failed to create muscle group: {ex.Message}", ResultType.Error);
        }
    }

    public async Task<Result> DeleteMuscleGroupAsync(int id)
    {
        try
        {
            await muscleGroupRepository.DeleteAsync(id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to delete muscle group: {ex.Message}", ResultType.Error);
        }
    }
}
