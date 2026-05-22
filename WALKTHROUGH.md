# RepVector: General System Walkthrough

This document provides a step-by-step walkthrough of how the RepVector system operates, tracing the flow of data across its architectural layers and explaining the responsibilities of each component.

## 1. The Architectural Layers

RepVector uses a strictly decoupled four-layer architecture. To understand the flow, we must first define the layers:

*   **WorkoutTracker.UI (Presentation)**: The Razor Pages frontend. It captures user intent and displays data.
*   **WorkoutTracker.Api (Service)**: The REST API entry point. It manages routing and initial request handling.
*   **WorkoutTracker.Logic (Business Logic)**: The brain of the system. It validates rules and checks permissions.
*   **WorkoutTracker.DAL (Data Access)**: The database engine. It executes raw SQL to read and write data.

---

## 2. Typical Flow: Starting a Workout

Let's trace what happens when a user navigates to a workout and clicks **"Start Workout"**.

### Phase A: The User Request (UI)
1.  The user is on the `Workouts/Details` page.
2.  They click the "Start Workout" button, which submits a form to the `OnPostStartSessionAsync` handler in `Details.cshtml.cs`.
3.  The UI handler retrieves the `UserId` from the current browser session.
4.  The UI handler calls the `SessionApiClient.StartSession(workoutId, userId)` method.
5.  The `ApiClient` sends an HTTP POST request to the API at `api/workoutsessions/start`.

### Phase B: The Gateway (API)
1.  The `UserContextFilter` intercepts the request, extracts the `X-User-Id` header, and populates the scoped `UserContext` object with the verified user.
2.  The `WorkoutSessionsController` receives the POST request and accesses the `CurrentUser` via the injected `UserContext`.
3.  The controller calls the `IWorkoutSessionService.StartSessionAsync()` method in the Logic layer.

### Phase C: The Decision (Logic)
1.  The `WorkoutSessionService` performs several checks:
    *   **Authorization**: It calls `IAuthorizationService.CanModifyWorkout()` to check permissions.
    *   **Validation**: It checks if there is already an active session for this user.
2.  If rules pass, the service generates a new `WorkoutSession` object and returns a `Result.Success(id)`.
3.  If a session exists, it returns a `Result.Conflict("AN_ACTIVE_SESSION_EXISTS")`.
4.  The service calls the `IWorkoutSessionRepository.CreateAsync()` method in the DAL for persistence.

### Phase D: Persistence (DAL)
1.  The `WorkoutSessionRepository` opens a connection to the MySQL database (injected via Primary Constructor).
2.  It executes a raw SQL command: `INSERT INTO workout_sessions (user_id, workout_id, started_at, status) VALUES (...)`.
3.  The database returns the newly generated ID for the session.
4.  The DAL returns this ID back up through the Logic layer.

### Phase E: UI Update
1.  The API controller uses `.ToActionResult()` to map the Service Result to the correct HTTP response (e.g., 200 OK or 409 Conflict).
2.  The UI receives the response.
3.  The Razor Page handler redirects the user to the `Workouts/Active` page.

---

## 3. Data Interaction: The "X-User-Id" Bridge

A critical part of the walkthrough is how the UI and API stay synchronized without sharing a database connection:

1.  **UI Session**: When you log in, the **UI project** stores your ID in its own memory (ASP.NET Session).
2.  **The Header**: Every time the UI needs data from the API, it attaches your ID to the `X-User-Id` header.
3.  **API Filtering**: The **API project** uses a global `UserContextFilter`. It doesn't "trust" the ID blindly; it uses its own `UserRepository` to fetch the user from the database to verify they exist.
4.  **Shared Context**: This verified user is stored in a scoped `UserContext` object, accessible by any controller or service during that specific request.
5.  **Clean Controllers**: Because the identification and authorization are handled by filters and dedicated services, the action methods in the controllers remain clean and focused only on mapping results to responses.

---

## 4. Why This Architecture?

By walking through this flow, several benefits become clear:

*   **Interchangeable UI**: Because the logic is in the `WorkoutTracker.Logic` project and exposed via the `WorkoutTracker.Api`, we could delete the Razor Pages project and replace it with a Mobile App tomorrow without changing a single line of database or business logic.
*   **Testability**: We can test the Logic layer in isolation by "faking" the DAL, ensuring our workout rules are perfect without ever needing a real database.
*   **Performance**: Because the DAL uses raw SQL, there is no hidden overhead. The "Walkthrough" from UI to Database happens in milliseconds.   
(Also because Raw SQL was a required part of the assignment)
