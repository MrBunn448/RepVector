using WorkoutTracker.Logic.Abstractions.Repositories;
using WorkoutTracker.Logic.Abstractions.Services;
using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Services;

public class WorkoutService(
    IWorkoutRepository workoutRepository, 
    IWorkoutExerciseRepository workoutExerciseRepository,
    IExerciseRepository exerciseRepository,
    IAuthorizationService auth) : IWorkoutService
{

    public Task<List<Workout>> GetAllByUserIdAsync(int userId)
        => workoutRepository.GetAllByUserIdAsync(userId);

    public Task<Workout?> GetByIdAsync(int workoutId)
        => workoutRepository.GetByIdAsync(workoutId);

    public async Task<Workout?> GetWorkoutDetailsAsync(int workoutId)
        => await workoutRepository.GetWorkoutWithExercisesAsync(workoutId);


    public async Task<Result<int>> CreateWorkoutAsync(Workout workout, User creator)
    {
        // 1. Validation:
        // Check role
        if (workout.IsPredefined && creator.Role != "Admin")
            return Result<int>.Forbidden("Only admins can create predefined workout templates.");
        // check workout type
        if (workout.IsPredefined)
        {
            workout.UserId = null; // null = Globaly viewable
            
            // Validation: global workouts must only contain global exercises
            if (workout.Exercises != null && workout.Exercises.Any())
            {
                foreach (var we in workout.Exercises)
                {
                    var exercise = await exerciseRepository.GetByIdAsync(we.ExerciseId);
                    if (exercise != null && !exercise.IsPredefined) //Personal exercise added, trow failure.
                        return Result<int>.Failure($"Template '{workout.Name}' cannot contain personal exercise '{exercise.Name}'.");
                }
            }
        }
        else
        {
            workout.UserId = creator.Id;
        }

        // 2. Data Persistence:
        workout.CreatedAt = DateTime.UtcNow; //Creation time
        var workoutId = await workoutRepository.CreateAsync(workout);

        // Save exercises:
        if (workout.Exercises != null)
        {
            foreach (var we in workout.Exercises)
            {
                we.WorkoutId = workoutId;
                await workoutExerciseRepository.CreateAsync(we);
            }
        }

        return Result<int>.Success(workoutId); 
    }

    public async Task<Result> UpdateWorkoutAsync(Workout workout, User editor)
    {
        var existing = await workoutRepository.GetWorkoutWithExercisesAsync(workout.Id);
        
        // 1. Centralized Authorization
        var authResult = auth.CanModifyWorkout(editor, existing);
        if (authResult.IsFailure) return authResult;

        // 2. Specific Validation for changing Predefined status
        if (workout.IsPredefined && editor.Role != "Admin")
            return Result.Forbidden("Only admins can make a workout predefined.");

        if (workout.IsPredefined)
        {
            workout.UserId = null;
            var exercisesToValidate = workout.Exercises ?? new List<WorkoutExercise>();
            foreach (var we in exercisesToValidate)
            {
                var exercise = await exerciseRepository.GetByIdAsync(we.ExerciseId);
                if (exercise != null && !exercise.IsPredefined)
                    return Result.Failure($"Predefined workout cannot contain personal exercise '{exercise.Name}'.");
            }
        }

        // 3. Save Changes & Sync nested exercises
        await workoutRepository.UpdateAsync(workout);

        var currentExercises = existing!.Exercises;
        var newExercises = workout.Exercises ?? new List<WorkoutExercise>();

        // Sync: Delete removed items
        foreach (var current in currentExercises)
        {
            if (!newExercises.Any(ne => ne.Id == current.Id))
                await workoutExerciseRepository.DeleteAsync(current.Id);
        }

        // Sync: Add or Update items
        foreach (var incoming in newExercises)
        {
            if (incoming.Id > 0)
            {
                var match = currentExercises.FirstOrDefault(c => c.Id == incoming.Id);
                if (match != null)
                {
                    match.TargetSets = incoming.TargetSets;
                    match.TargetReps = incoming.TargetReps;
                    match.TargetRpe = incoming.TargetRpe;
                    match.SortOrder = incoming.SortOrder;
                    match.ExerciseId = incoming.ExerciseId;
                    await workoutExerciseRepository.UpdateAsync(match);
                }
            }
            else
            {
                incoming.WorkoutId = workout.Id;
                await workoutExerciseRepository.CreateAsync(incoming);
            }
        }

        return Result.Success();
    }

    public async Task<Result> DeleteWorkoutAsync(int workoutId, User deleter)
    {
        var existing = await workoutRepository.GetByIdAsync(workoutId);
        
        // Centralized Authorization
        var authResult = auth.CanModifyWorkout(deleter, existing);
        if (authResult.IsFailure) return authResult;

        await workoutRepository.DeleteAsync(workoutId);
        return Result.Success();
    }
}
