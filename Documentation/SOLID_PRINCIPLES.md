# SOLID Principles in RepVector

This document explains how the SOLID principles are applied across the RepVector codebase with specific examples for each principle.

## Single Responsibility Principle (SRP)

General Description: A class should have one, and only one, reason to change. This means every class should have a single focused purpose or responsibility within the system.

Specific Example: The AuthorizationService class.
In RepVector, business services like WorkoutService or ExerciseService do not contain the logic to determine if a user has permission to perform an action. Instead, this logic is centralized in the AuthorizationService. This separation ensures that if the rules for permissions change (for example, adding a new role), only the AuthorizationService needs to be modified, while the business logic remains untouched.

## Open/Closed Principle (OCP)

General Description: Software entities should be open for extension but closed for modification. You should be able to add new functionality without changing existing code.

Specific Example: Repository Interfaces.
All data access logic is defined through interfaces like IWorkoutRepository. The WorkoutService depends on this interface rather than a concrete SQL implementation. If the system needs to switch from raw MySQL queries to a different database or an ORM like Entity Framework, a new repository class can be created that implements the same interface. The WorkoutService will work with the new repository without needing any code changes.

## Liskov Substitution Principle (LSP)

General Description: Objects of a superclass should be replaceable with objects of its subclasses without breaking the application.

Specific Example: Result and Result of T.
The system uses a custom Result class to handle operation outcomes. The Result of T class inherits from the base Result class. Any method that expects a standard Result object can successfully receive and process a Result of T object because the subclass maintains the behavioral contract of the parent (providing IsSuccess and ErrorMessage properties).

## Interface Segregation Principle (ISP)

General Description: No client should be forced to depend on methods it does not use. This is achieved by creating small, specific interfaces rather than large, general ones.

Specific Example: Granular Service Interfaces.
Instead of having a single IDataService that handles everything in the system, RepVector uses specific interfaces like IAuthService, IExerciseService, and IWorkoutSessionService. A controller that only needs to manage exercises only depends on IExerciseService and is not exposed to unrelated logic like user registration or session tracking.

## Dependency Inversion Principle (DIP)

General Description: High level modules should not depend on low level modules. Both should depend on abstractions.

Specific Example: Constructor Injection in Controllers.
The ExercisesController does not create a new instance of ExerciseService inside its constructor. Instead, it accepts an IExerciseService interface. The actual implementation is "injected" by the ASP.NET Core dependency injection container at runtime. This removes the tight coupling between the API layer and the logic implementation, making the code more flexible and easier to unit test with mock objects.
