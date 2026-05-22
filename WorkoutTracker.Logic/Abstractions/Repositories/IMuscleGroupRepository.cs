using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Abstractions.Repositories;

/// <summary>
/// Defines the contract for data access operations related to muscle groups.
/// </summary>
public interface IMuscleGroupRepository
{
    /// <summary>
    /// Retrieves all muscle groups defined in the system.
    /// </summary>
    /// <returns>A list of muscle group entities.</returns>
    Task<List<MuscleGroup>> GetAllAsync();

    /// <summary>
    /// Retrieves a specific muscle group by its ID.
    /// </summary>
    /// <param name="muscleGroupId">The ID of the muscle group.</param>
    /// <returns>The muscle group entity if found, otherwise null.</returns>
    Task<MuscleGroup?> GetByIdAsync(int muscleGroupId);

    /// <summary>
    /// Persists a new muscle group in the database.
    /// </summary>
    /// <param name="muscleGroup">The muscle group data to save.</param>
    /// <returns>The ID of the newly created muscle group.</returns>
    Task<int> CreateAsync(MuscleGroup muscleGroup);

    /// <summary>
    /// Updates an existing muscle group record.
    /// </summary>
    /// <param name="muscleGroup">The muscle group entity with updated values.</param>
    Task UpdateAsync(MuscleGroup muscleGroup);

    /// <summary>
    /// Removes a muscle group record from the database.
    /// </summary>
    /// <param name="muscleGroupId">The ID of the muscle group to delete.</param>
    Task DeleteAsync(int muscleGroupId);
}
