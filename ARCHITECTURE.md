# RepVector Technical Documentation

This document provides a deep dive into the architecture, data flow, and security model of the RepVector workout tracking system.

## 🏗 Architectural Overview

RepVector is built using a **Strict 4-Layer Architecture** to ensure separation of concerns and maintainability.

### 1. Presentation Layer (`WorkoutTracker.UI`)
*   **Technology**: ASP.NET Core Razor Pages.
*   **Role**: Handles user interaction, input validation (client-side), and state management via HTTP sessions.
*   **Communication**: It never touches the database directly. It consumes the API via strongly-typed `ApiClient` services using `HttpClient`.

### 2. Service Layer (`WorkoutTracker.Api`)
*   **Technology**: ASP.NET Core Web API.
*   **Role**: Acts as the gatekeeper. It handles request routing, model binding, and identification.
*   **Composition Root**: This is where all dependencies (Services and Repositories) are registered and wired together.

### 3. Business Logic Layer (`WorkoutTracker.Logic`)
*   **Technology**: Plain Old CLR Objects (POCO) Services.
*   **Role**: Enforces domain rules.
*   **Authorization**: Uses a centralized `IAuthorizationService` to handle "Can this user do X to Y?" logic across the system, removing boilerplate from business services.
*   **Result Pattern**: Services return a `Result` or `Result<T>` object instead of throwing exceptions, providing "Plain English" feedback to the API layer.
*   **Abstractions**: Defines interfaces for the Data Access Layer, enabling Dependency Inversion.

### 4. Data Access Layer (`WorkoutTracker.DAL`)
*   **Technology**: MySqlConnector (Raw SQL).
*   **Role**: Performs high-performance CRUD operations. It maps database rows to the Shared Models.
*   **No ORM**: Uses optimized, handwritten SQL queries for maximum transparency and performance.
*   **Modern DI**: Uses C# 12 Primary Constructors to inject `DbConnectionFactory` with zero boilerplate.

---

## 🔄 Data Flow: The "Active Session" Lifecycle

To understand how data moves through the system, consider the flow of logging a set during a workout:

1.  **UI**: The user enters weight/reps in `Active.cshtml`. Clicking "Log" triggers the `logSet(btn)` JavaScript function.
2.  **UI Service**: A `POST` request is sent to the `OnPostLogSetAsync` handler in `Active.cshtml.cs`.
3.  **API Call**: The UI's `SessionApiClient` sends a JSON payload to `POST /api/workoutsessions/{id}/log-set`. It includes the `X-User-Id` header from the current session.
4.  **API Filter**: The `UserContextFilter` intercepts the request, validates the `X-User-Id` header, and populates a scoped `UserContext` object.
5.  **API Controller**: `WorkoutSessionsController` (inheriting from `BaseWorkoutController`) receives the request. It passes the `CurrentUser` from the context to the service.
6.  **Logic Layer**: `WorkoutSessionService` calls `IAuthorizationService` to verify permissions and returns a `Result`.
7.  **DAL**: If authorized, `WorkoutSessionRepository` executes an `INSERT INTO workout_set_logs ...` statement.
8.  **Response**: The controller uses `.ToActionResult()` to map the `Result` to the appropriate HTTP status code (200 OK, 403 Forbidden, etc.), which propagates back to the UI.

---

## 🔐 Accounts & Security Model

### Authentication & Identification
RepVector uses a **Filter-Driven Identification** model:
*   **Login**: Users authenticate against `api/auth/login`. On success, the UI stores the `UserId` and `UserRole` in an encrypted server-side ASP.NET Session.
*   **API Requests**: Every subsequent API call from the UI includes the `X-User-Id` header. 
*   **UserContextFilter**: This global action filter fetches the `User` object once per request and stores it in a scoped `UserContext`.
*   **BaseWorkoutController**: Provides a `protected User? CurrentUser` property, making user identification "invisible" noise in the actual action methods.

### Role-Based Access Control (RBAC)
Two roles exist: `User` and `Admin`.
*   **Standard User**: Can CRUD their own Workouts, Exercises, and Sessions. They have read-only access to "Global Templates" (where `IsPredefined = true`).
*   **Admin**: Has full CRUD access to all data, including the ability to create and modify "RepVector Official" global templates and exercises.
*   **Enforcement**: All logic is centralized in `AuthorizationService.cs` to ensure consistency.

---

## 📡 API Endpoint Reference

### Workout Management (`/api/workouts`)
| Endpoint | Method | Description |
|----------|--------|-------------|
| `GET /` | GET | Returns all workouts owned by the user + global templates. |
| `GET /{id}` | GET | Returns full details of a specific workout, including its exercises. |
| `POST /` | POST | Creates a new workout template. |
| `PUT /{id}` | PUT | Updates workout metadata or the list of exercises (for owners/admins). |
| `DELETE /{id}` | DELETE | Removes a workout template. |

### Exercise Catalog (`/api/exercises`)
| Endpoint | Method | Description |
|----------|--------|-------------|
| `GET /` | GET | Returns the combined catalog of global and personal exercises. |
| `POST /` | POST | Creates a new personal exercise. |

### Workout Sessions (`/api/workoutsessions`)
| Endpoint | Method | Description |
|----------|--------|-------------|
| `GET /active` | GET | Finds the currently ongoing session for the authenticated user. |
| `POST /start` | POST | Initializes a new session from a workout template. |
| `POST /{id}/log` | POST | Records a performance set (weight/reps/RPE). |
| `PUT /{id}/status`| PUT | Updates session status (e.g., to 'completed'). |

---

## 📦 Shared Models
All layers depend on `WorkoutTracker.Models`. Key entities include:
*   **Workout**: The template definition.
*   **Exercise**: A movement definition (e.g., "Bench Press").
*   **WorkoutExercise**: The bridge joining a workout to an exercise with target sets/reps.
*   **WorkoutSession**: A live instance of a workout.
*   **WorkoutSetLog**: The actual performance data recorded during a session.
