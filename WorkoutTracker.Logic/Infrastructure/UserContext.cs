using WorkoutTracker.Models;

namespace WorkoutTracker.Logic.Infrastructure;


public class UserContext
{
    public User? User { get; set; }
    
    public bool IsAuthenticated => User != null;
}
