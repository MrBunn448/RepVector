# RepVector - Developer & AI Extension Guide

This document provides essential information for developers and AI agents to understand, maintain, and extend the RepVector workout tracking system.

## System Architecture

RepVector follows a strictly decoupled **4-Layer Architecture**:

1.  **WorkoutTracker.UI**: Razor Pages frontend. Communicates with the API via dedicated HTTP clients. No direct DB access.
2.  **WorkoutTracker.Api**: ASP.NET Core Web API. Exposes REST endpoints. Handles HTTP request/response mapping and calls the Logic layer.
3.  **WorkoutTracker.Logic**: Business Logic Layer. Contains services that enforce business rules, validation, and authorization.
4.  **WorkoutTracker.DAL**: Data Access Layer. Uses `MySqlConnector` for raw SQL execution. No ORM (like Entity Framework) is used.

**Shared Models**: `WorkoutTracker.Models` contains the POCOs used across all layers.

---

## User Roles & Permissions

The system currently supports two roles: `User` and `Admin`.

| Feature | Normal User | Admin User |
|---------|-------------|------------|
| **Own Workouts** | Full CRUD | Full CRUD |
| **Global Templates**| View Only | Full CRUD |
| **Own Exercises** | Full CRUD | Full CRUD |
| **Global Exercises**| View Only | Full CRUD |
| **Sessions** | Full CRUD (Own) | Full CRUD (Own) |

### Authorization Implementation
Authorization is centralized in the `IAuthorizationService`. Instead of manual checks in every service method, services delegate the "Can this user modify this?" logic to the auth service. 

Example:
```csharp
var authResult = auth.CanModifyWorkout(editor, existing);
if (authResult.IsFailure) return authResult;
```

---

## 📊 Data Ownership Model

### Exercises
- **Predefined (`IsPredefined = true`)**: `UserId` is `NULL`. Visible to ALL users. Editable only by Admins.
- **Personal (`IsPredefined = false`)**: `UserId` is set to the creator. Visible and editable ONLY by the owner.

### Workouts
- **Predefined (`IsPredefined = true`)**: `UserId` is `NULL`. Visible to ALL users as "Templates". Editable only by Admins.
- **Personal (`IsPredefined = false`)**: `UserId` is set to the owner. Visible and editable ONLY by the owner.

---

## 🛠 Modern C# Patterns & Coding Standards

To keep the codebase clean and maintainable, we leverage several modern C# features:

### 1. Result Pattern
Instead of throwing exceptions for business logic failures (like "Not Found" or "Forbidden"), services return a `Result` or `Result<T>`. 
- **Benefit**: The API Layer can map these results to HTTP status codes using the `.ToActionResult()` extension method, leading to extremely concise controllers.

### 2. Primary Constructors (C# 12)
We use Primary Constructors for dependency injection in all Services and Repositories.
- **Benefit**: Reduces boilerplate code (no need for private fields or explicit constructor assignments).

### 3. Action Filters & UserContext
The `UserContextFilter` automatically populates a scoped `UserContext` object using the `X-User-Id` header.
- **Benefit**: Action methods don't need to manually fetch the current user; they can access it via the `CurrentUser` property in `BaseWorkoutController`.

---

## Other

- **Adding a New Repository**: Define the interface in `WorkoutTracker.DAL/Repositories`, implement it, and register it in `WorkoutTracker.Api/Program.cs`.
- **API Communication**: Always use the `X-User-Id` header when calling the API from the UI. This is currently how the API identifies the requester.
- **Raw SQL**: When updating queries, remember to handle `NULL` values using `DBNull.Value` in `MySqlCommand` parameters.
- **UI Styling**: Use Bootstrap 5 classes. Icons are provided by Bootstrap Icons (`bi-`).
