# RepVector Testing Documentation

This document outlines the testing strategy, current coverage, and infrastructure used to ensure the reliability and integrity of the RepVector system.

## 1. Testing Strategy Overview

RepVector utilizes a two-tier testing strategy to balance development speed with system reliability.

### Tier 1: Unit Testing (`WorkoutTracker.Logic.Tests`)
*   **Focus:** Core business logic, authorization rules, and validation.
*   **Infrastructure:** Uses **Moq** to bypass the database entirely.
*   **Benefit:** Extremely fast; allows for testing complex scenarios (like admin-only rules) without database setup.

### Tier 2: Integration Testing (`WorkoutTracker.IntegrationTests`)
*   **Focus:** Entire system stack, including API routing, SQL query correctness, and database constraints.
*   **Infrastructure:** Uses the real **`repvectortest`** database with a **Zero-Mock Policy**.
*   **Benefit:** Guarantees that the code actually works when connected to MySQL.

### Key Principles:
*   **Dynamic Data Flow:** Integration tests do not use hardcoded IDs. They follow a "Create -> Capture ID -> Use" pattern.
*   **Automated Cleanup:** Every test cleans its own data from `repvectortest` via cascading deletes.
*   **Coverage Tracking:** **Coverlet** is used to monitor and report on how much of the application is verified by these two tiers.

---

## 2. Current Test Coverage & Results

| Test Category | Scenario | Requirement Ref | Status |
| :--- | :--- | :--- | :--- |
| **Authentication** | User Registration & Login Flow | FR-1.1, FR-1.2 | Success |
| **Workout Lifecycle**| Template Creation -> Live Session -> History | FR-3.1, FR-4.1, FR-5.1 | Success |
| **Exercise Management**| Custom Exercise Creation & Data Isolation | FR-2.2 | Success |
| **Template Details** | Deep-Join Fetching (Workouts + Exercises) | FR-3.2 | Success |
| **Error Handling** | 404 Not Found for non-existent resources | NFR-5 | Success |

---

## 3. Test Infrastructure

### Test Database: `repvectortest`
The system is configured via `WorkoutTracker.IntegrationTests/appsettings.json` to target the local test database. This ensures that the production database (`repvectorprod`) is never touched during automated runs.

### Test Factory: `WorkoutTrackerWebApplicationFactory`
This class serves as the "Composition Root" for tests. It:
1.  Loads the test-specific `appsettings.json`.
2.  Bootstraps the API in memory using the real `Program.cs` logic.
3.  Ensures real repositories are used for 100% stack coverage.

---

## 4. Cleanup & Maintenance

To maintain a clean database and prevent test interference, RepVector implements an automatic cleanup mechanism using the xUnit `IAsyncLifetime` interface.

### The Cleanup Process:
1.  **Tracking:** During a test, every newly created `UserId` is added to a private `_testUserIds` list.
2.  **Surgical Deletion:** In the `DisposeAsync` method (executed after every test), the system:
    *   Deletes all **Workouts** owned by the test user (this triggers a cascading delete of `workout_exercises`).
    *   Deletes the **User** record.
3.  **Cascading Integrity:** Because of the `ON DELETE CASCADE` constraints in the master schema, deleting the user automatically removes all their:
    *   Sessions (`workout_sessions`)
    *   Set Logs (`workout_set_logs`)
    *   Private Exercises (`exercises`)
    *   Preferences (`user_preferences`)

This ensures that even if a test fails midway, the database is reset for the next run. And there is no buildup of test data over time, keeping the environment clean and reliable.

---

## 5. How to Run Tests

### Via Visual Studio:
1.  Open **Test Explorer** Via UI or (`Ctrl + E, T`).
2.  Click **Run All** or choose the specfic tests you want to run..
3.  Results are displayed with green checkmarks for successes.

### Via Command Line:
Navigate to the project root and run:
```powershell
dotnet test WorkoutTracker.IntegrationTests/WorkoutTracker.IntegrationTests.csproj
```
