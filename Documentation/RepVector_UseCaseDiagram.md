# RepVector Use Case Diagram

This diagram visualizes the interactions between users and the system, covering 100% of the application's core functionality.

```mermaid
graph LR
    %% Actors
    User((Standard User))
    Admin((Administrator))

    subgraph "RepVector System"
        
        subgraph "User Management"
            UC11(["UC-1.1 Register Account"])
            UC12(["UC-1.2 Login"])
            UC13(["UC-1.4 Logout"])
            UC14(["UC-1.5 Manage Preferences"])
            UC15(["UC-1.6 Register Administrator"])
        end

        subgraph "Exercise Catalog"
            UC21(["UC-2.1 View Exercise Library"])
            UC22(["UC-2.2 Create Private Exercise"])
            UC23(["UC-2.3 Edit Private Exercise"])
            UC24(["UC-2.4 Delete Private Exercise"])
            UC25(["UC-2.5 Manage Global Catalog (CRUD)"])
        end

        subgraph "Workout Templates"
            UC31(["UC-3.1 View All Templates"])
            UC32(["UC-3.2 Create Personal Template"])
            UC33(["UC-3.3 Edit Personal Template"])
            UC34(["UC-3.4 Delete Personal Template"])
            UC35(["UC-3.5 Manage Global Templates (CRUD)"])
        end

        subgraph "Training Sessions"
            UC41(["UC-4.1 Start Training Session"])
            UC42(["UC-4.2 Log Set Performance"])
            UC43(["UC-4.3 Finish Workout"])
            UC44(["UC-4.4 Cancel Active Workout"])
        end

        subgraph "Progress & History"
            UC51(["UC-5.1 View Session History"])
            UC52(["UC-5.2 View Session Details"])
            UC53(["UC-5.3 Delete Session Log"])
        end
    end

    %% Standard User Connections
    User --- UC11
    User --- UC12
    User --- UC13
    User --- UC14
    User --- UC21
    User --- UC22
    User --- UC23
    User --- UC24
    User --- UC31
    User --- UC32
    User --- UC33
    User --- UC34
    User --- UC41
    User --- UC42
    User --- UC43
    User --- UC44
    User --- UC51
    User --- UC52
    User --- UC53

    %% Admin Connections
    Admin --- UC15
    Admin --- UC25
    Admin --- UC35
    
    %% Admin inherits User capabilities
    Admin -.-> User

    %% Styling for visual clarity (Dark Theme Friendly)
    classDef actor fill:#4a148c,stroke:#e1bee7,stroke-width:2px,color:#fff
    classDef usecase fill:#1b5e20,stroke:#c8e6c9,stroke-width:1px,color:#fff
    classDef system fill:#212121,stroke:#616161,color:#fff
    class User,Admin actor
    class UC11,UC12,UC13,UC14,UC15,UC21,UC22,UC23,UC24,UC25,UC31,UC32,UC33,UC34,UC35,UC41,UC42,UC43,UC44,UC51,UC52,UC53 usecase
    class RepVector_System,User_Management,Exercise_Catalog,Workout_Templates,Training_Sessions,Progress_&_History system
```

### Actors Definition
*   **Standard User**: A person who uses the application to track their own personal fitness data and routines.
*   **Administrator**: A person with elevated privileges who can manage the global content available to all users. Inherits all capabilities of the Standard User.
