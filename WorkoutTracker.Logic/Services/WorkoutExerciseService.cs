using WorkoutTracker.Logic.Abstractions.Repositories;
using WorkoutTracker.Logic.Abstractions.Services;
using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Services;

public class WorkoutExerciseService(
    IWorkoutExerciseRepository workoutExerciseRepository, 
    IWorkoutRepository workoutRepository,
    IAuthorizationService auth) : IWorkoutExerciseService
{
    /// Retrieves all exercise links for a specific workout.
    public Task<List<WorkoutExercise>> GetByWorkoutIdAsync(int workoutId)
        => workoutExerciseRepository.GetByWorkoutIdAsync(workoutId);

    /// Retrieves a specific workout-exercise association by its identifier.
    public Task<WorkoutExercise?> GetByIdAsync(int workoutExerciseId)
        => workoutExerciseRepository.GetByIdAsync(workoutExerciseId);

    /// Adds an exercise to a workout template. Validates that the user has permission to modify the workout.
    public async Task<Result> AddAsync(WorkoutExercise workoutExercise, User user)
    {
        var workout = await workoutRepository.GetByIdAsync(workoutExercise.WorkoutId);
        
        // 1. Centralized permission check
        var authResult = auth.CanModifyWorkout(user, workout);
        if (authResult.IsFailure) return authResult;

        await workoutExerciseRepository.CreateAsync(workoutExercise);
        return Result.Success();
    }

    /// Updates an existing exercise link within a workout. Validates permissions for the parent workout.
    public async Task<Result> UpdateAsync(WorkoutExercise workoutExercise, User user)
    {
        var existingLink = await workoutExerciseRepository.GetByIdAsync(workoutExercise.Id);
        if (existingLink == null) return Result.NotFound("Workout-Exercise link not found.");

        var workout = await workoutRepository.GetByIdAsync(existingLink.WorkoutId);
        
        // 1. Centralized permission check
        var authResult = auth.CanModifyWorkout(user, workout);
        if (authResult.IsFailure) return authResult;

        // 2. Perform the action
        await workoutExerciseRepository.UpdateAsync(workoutExercise);
        return Result.Success();
    }

    /// Removes an exercise from a workout template. Validates permissions for the parent workout.
    public async Task<Result> DeleteAsync(int workoutExerciseId, User user)
    {
        var existingLink = await workoutExerciseRepository.GetByIdAsync(workoutExerciseId);
        if (existingLink == null) return Result.NotFound();

        var workout = await workoutRepository.GetByIdAsync(existingLink.WorkoutId);
        
        // 1. Centralized permission check
        var authResult = auth.CanModifyWorkout(user, workout);
        if (authResult.IsFailure) return authResult;

        // 2. Perform the action
        await workoutExerciseRepository.DeleteAsync(workoutExerciseId);
        return Result.Success();
    }
}