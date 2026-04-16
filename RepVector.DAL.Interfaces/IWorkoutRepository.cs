using System;
using System.Collections.Generic;
using System.Text;

using RepVector.Models;

namespace RepVector.DAL.Interfaces;

public interface IWorkoutRepository
{
    IEnumerable<Workout> GetAll();
    Workout? GetById(int id);
}