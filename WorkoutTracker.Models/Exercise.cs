using System;
using System.Collections.Generic;
using System.Text;

namespace WorkoutTracker.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Reps { get; set; }
        public int Sets { get; set; }

        public int WorkoutId { get; set; }
    }
}
