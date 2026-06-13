using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Abstractions.Services;

/// Centralized service to handle permission logic across the system.
/// This removes repetitive authorization checks from business services.
public interface IAuthorizationService
{
    /// Checks if a user has permission to modify a specific workout template.
    /// <param name="user">The user attempting the modification.</param>
    /// <param name="workout">The workout template to be modified.</param>
    /// <returns>A successful Result if authorized, otherwise a failure Result with Forbidden or NotFound type.</returns>
    Result CanModifyWorkout(User user, Workout? workout);
    
    /// Checks if a user has permission to modify a specific exercise.
    /// <param name="user">The user attempting the modification.</param>
    /// <param name="exercise">The exercise to be modified.</param>
    /// <returns>A successful Result if authorized, otherwise a failure Result with Forbidden or NotFound type.</returns>
    Result CanModifyExercise(User user, Exercise? exercise);

    /// Checks if a user has permission to modify a specific workout session.
    /// <param name="user">The user attempting the modification.</param>
    /// <param name="session">The workout session to be modified.</param>
    /// <returns>A successful Result if authorized, otherwise a failure Result with Forbidden or NotFound type.</returns>
    Result CanModifySession(User user, WorkoutSession? session);

    /// Checks if a user has permission to modify preferences for a target user.
    /// <param name="user">The user attempting the modification.</param>
    /// <param name="targetUserId">The ID of the user whose preferences are being modified.</param>
    /// <returns>A successful Result if authorized, otherwise a failure Result with Forbidden type.</returns>
    Result CanModifyPreference(User user, int targetUserId);
}
