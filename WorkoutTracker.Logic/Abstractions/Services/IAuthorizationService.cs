using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Abstractions.Services;

/// <summary>
/// Centralized service to handle permission logic across the system.
/// This removes repetitive authorization checks from business services.
/// </summary>
public interface IAuthorizationService
{
    /// <summary>
    /// Checks if a user has permission to modify a specific workout template.
    /// </summary>
    /// <param name="user">The user attempting the modification.</param>
    /// <param name="workout">The workout template to be modified.</param>
    /// <returns>A successful Result if authorized, otherwise a failure Result with Forbidden or NotFound type.</returns>
    Result CanModifyWorkout(User user, Workout? workout);
    
    /// <summary>
    /// Checks if a user has permission to modify a specific exercise.
    /// </summary>
    /// <param name="user">The user attempting the modification.</param>
    /// <param name="exercise">The exercise to be modified.</param>
    /// <returns>A successful Result if authorized, otherwise a failure Result with Forbidden or NotFound type.</returns>
    Result CanModifyExercise(User user, Exercise? exercise);

    /// <summary>
    /// Checks if a user has permission to modify a specific workout session.
    /// </summary>
    /// <param name="user">The user attempting the modification.</param>
    /// <param name="session">The workout session to be modified.</param>
    /// <returns>A successful Result if authorized, otherwise a failure Result with Forbidden or NotFound type.</returns>
    Result CanModifySession(User user, WorkoutSession? session);

    /// <summary>
    /// Checks if a user has permission to modify preferences for a target user.
    /// </summary>
    /// <param name="user">The user attempting the modification.</param>
    /// <param name="targetUserId">The ID of the user whose preferences are being modified.</param>
    /// <returns>A successful Result if authorized, otherwise a failure Result with Forbidden type.</returns>
    Result CanModifyPreference(User user, int targetUserId);
}
