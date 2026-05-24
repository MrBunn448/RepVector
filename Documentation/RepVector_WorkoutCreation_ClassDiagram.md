# Feature Class Diagram: Workout Creation

This diagram represents the "Vertical Slice" of the application for the **Workout Creation** feature, using stereotypes for cleaner visualization of layers.

```mermaid
classDiagram
    direction TB

    %% --- Tier 1: Presentation Layer ---
    class CreateModel {
        -WorkoutApiClient _api
        +Workout Workout
        +OnPostAsync() Task
    }
    class WorkoutApiClient {
        -HttpClient _httpClient
        +CreateWorkout(...) Task
    }

    %% --- Tier 2: API Gateway ---
    class WorkoutsController {
        -IWorkoutService _workoutService
        +Create(Workout workout) Task
    }

    %% --- Tier 3: Business Logic ---
    class WorkoutService {
        <<IWorkoutService>>
        -IWorkoutRepository _workoutRepo
        -IAuthorizationService _auth
        +CreateWorkoutAsync(Workout w, User c) Task
    }

    %% --- Tier 4: Data Access ---
    class WorkoutRepository {
        <<IWorkoutRepository>>
        -DbConnectionFactory _db
        +CreateAsync(Workout workout) Task
    }

    %% --- Cross-Cutting ---
    class Workout {
        +int Id
        +string Name
        +int? UserId
    }

    %% --- Interactions ---
    CreateModel ..> WorkoutApiClient : uses
    WorkoutApiClient ..> WorkoutsController : HTTP POST
    WorkoutsController ..> WorkoutService : injected (DI)
    WorkoutService ..> WorkoutRepository : injected (DI)

    %% Styling
    classDef ui fill:#4a148c,stroke:#e1bee7,color:#fff
    classDef api fill:#1a237e,stroke:#9fa8da,color:#fff
    classDef logic fill:#1b5e20,stroke:#c8e6c9,color:#fff
    classDef dal fill:#3e2723,stroke:#d7ccc8,color:#fff
    classDef model fill:#212121,stroke:#616161,color:#fff

    %% Apply styles individually for maximum compatibility
    class CreateModel ui
    class WorkoutApiClient ui
    class WorkoutsController api
    class WorkoutService logic
    class WorkoutRepository dal
    class Workout model
```

### Architectural Notes:
*   **Stereotypes**: Classes are marked with stereotypes (e.g., `<<IWorkoutService>>`) to show they fulfill a specific interface contract without needing separate boxes.
*   **Vertical Slice**: Traces the flow from UI -> API -> Logic -> DAL for a single requirement.
*   **Color-Coding**: 
    *   **Purple**: Presentation Layer
    *   **Blue**: API Gateway
    *   **Green**: Business Logic
    *   **Brown**: Data Access
    *   **Dark Grey**: Shared Models
