using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Abstractions.Repositories;

/// Defines the contract for data access operations related to muscle groups.
public interface IMuscleGroupRepository
{
    /// Retrieves all muscle groups defined in the system.
    /// <returns>A list of muscle group entities.</returns>
    Task<List<MuscleGroup>> GetAllAsync();

    /// Retrieves a specific muscle group by its ID.
    /// <param name="muscleGroupId">The ID of the muscle group.</param>
    /// <returns>The muscle group entity if found, otherwise null.</returns>
    Task<MuscleGroup?> GetByIdAsync(int muscleGroupId);

    /// Persists a new muscle group in the database.
    /// <param name="muscleGroup">The muscle group data to save.</param>
    /// <returns>The ID of the newly created muscle group.</returns>
    Task<int> CreateAsync(MuscleGroup muscleGroup);

    /// Updates an existing muscle group record.
    /// <param name="muscleGroup">The muscle group entity with updated values.</param>
    Task UpdateAsync(MuscleGroup muscleGroup);

    /// Removes a muscle group record from the database.
    /// <param name="muscleGroupId">The ID of the muscle group to delete.</param>
    Task DeleteAsync(int muscleGroupId);
}
