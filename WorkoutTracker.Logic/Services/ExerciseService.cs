using WorkoutTracker.Logic.Abstractions.Repositories;
using WorkoutTracker.Logic.Abstractions.Services;
using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Services;

public class ExerciseService(
    IExerciseRepository exerciseRepository,
    IAuthorizationService auth) : IExerciseService
{
    /// Retrieves all exercises available to a specific user, including their own and global ones.
    public Task<List<Exercise>> GetAllAsync(int userId)
        => exerciseRepository.GetAllAsync(userId);


    /// Retrieves a specific exercise by its id
    public Task<Exercise?> GetByIdAsync(int exerciseId)
        => exerciseRepository.GetByIdAsync(exerciseId);

    /// Creates a new exercise. Only admins can create predefined exercises.
    public async Task<Result<int>> CreateAsync(Exercise exercise, User creator)
    {
        if (exercise.IsPredefined && creator.Role != "Admin")
            return Result<int>.Forbidden("Only admins can create predefined movements.");

        if (exercise.IsPredefined)
            exercise.UserId = null;
        else
            exercise.UserId = creator.Id;

        exercise.CreatedAt = DateTime.UtcNow;
        var id = await exerciseRepository.CreateAsync(exercise);
        return Result<int>.Success(id);
    }

    /// Updates an existing exercise. Validates ownership or admin role via AuthorizationService.
    public async Task<Result> UpdateAsync(Exercise exercise, User editor)
    {
        var existing = await exerciseRepository.GetByIdAsync(exercise.Id);
        
        var authResult = auth.CanModifyExercise(editor, existing);
        if (authResult.IsFailure) return authResult;

        if (exercise.IsPredefined && editor.Role != "Admin")
            return Result.Forbidden("Only admins can make a movement predefined.");

        // Keep original creation date
        exercise.CreatedAt = existing!.CreatedAt;

        await exerciseRepository.UpdateAsync(exercise);
        return Result.Success();
    }

    /// Deletes an exercise. Validates ownership or admin role via AuthorizationService.
    public async Task<Result> DeleteAsync(int exerciseId, User deleter)
    {
        var existing = await exerciseRepository.GetByIdAsync(exerciseId);
        
        var authResult = auth.CanModifyExercise(deleter, existing);
        if (authResult.IsFailure) return authResult;

        await exerciseRepository.DeleteAsync(exerciseId);
        return Result.Success();
    }
}
