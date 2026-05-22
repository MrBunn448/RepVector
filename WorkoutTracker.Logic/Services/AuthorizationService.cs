using WorkoutTracker.Models;
using WorkoutTracker.Logic.Abstractions.Services;

namespace WorkoutTracker.Logic.Services;

/// Implementation of the permission logic used across the system to ensure role-based access control.
public class AuthorizationService : IAuthorizationService
{

    /// Evaluates if a user is permitted to modify a specific workout template.
    public Result CanModifyWorkout(User user, Workout? workout)
    {
        if (workout == null) return Result.NotFound("Workout not found.");

        // Admins can touch everything
        if (user.Role == "Admin") return Result.Success();

        // Standard users cannot modify official global templates
        if (workout.IsPredefined) 
            return Result.Forbidden("Only administrators can modify official global templates.");

        // Standard users can only modify their own personal workouts
        if (workout.UserId != user.Id)
            return Result.Forbidden("You do not have permission to modify this workout.");

        return Result.Success();
    }


    /// Evaluates if a user is permitted to modify a specific exercise.
    public Result CanModifyExercise(User user, Exercise? exercise)
    {
        if (exercise == null) return Result.NotFound("Exercise not found.");

        if (user.Role == "Admin") return Result.Success();

        if (exercise.IsPredefined)
            return Result.Forbidden("Only administrators can modify official global exercises.");

        if (exercise.UserId != user.Id)
            return Result.Forbidden("You do not have permission to modify this exercise.");

        return Result.Success();
    }

    /// Evaluates if a user is permitted to modify a specific workout session.
    public Result CanModifySession(User user, WorkoutSession? session)
    {
        if (session == null) return Result.NotFound("Workout session not found.");

        // Admins can manage all sessions
        if (user.Role == "Admin") return Result.Success();

        // Users can only manage their own sessions
        if (session.UserId != user.Id)
            return Result.Forbidden("You do not have permission to modify this session.");

        return Result.Success();
    }

    /// Evaluates if a user is permitted to modify preferences for a specific target user.
    public Result CanModifyPreference(User user, int targetUserId)
    {
        // Admins can modify anyone's preferences
        if (user.Role == "Admin") return Result.Success();

        // Users can only modify their own preferences
        if (user.Id != targetUserId)
            return Result.Forbidden("You do not have permission to modify preferences for this user.");

        return Result.Success();
    }
}
