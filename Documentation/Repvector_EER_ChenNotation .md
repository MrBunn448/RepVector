NOTE: In Visual Studio Code 1.121.0 and later, you can view this mermaid diagram directly in Visual Studio code. A community extention for viewing was intergrated into visual studio itself.

## Design Reasoning (EER)
*   **Specialization (ISA)**: Used for both **Users** (Admin vs.
Standard) and **Exercises/Workouts** (System vs. Custom). This allows
the application to handle global master-data and private user-data
within the same logical structure.
*   **Weak Entity (User_Preferences)**: Labeled as weak because it
shares its identity with the `User`. It has no independent Primary Key;
it uses the `user_id` to exist, ensuring a strict 1:1 dependency.
*   **Identifying Relationship**: The double-diamond `Has` relationship
indicates that if a User is deleted, their preferences must also be
purged (Existence Dependency).
*   **Cardinality (1:N and M:N)**: Clearly distinguishes that while a
User owns many Sessions (1:N), the relationship between Workouts and
Exercises is Many-to-Many (M:N), requiring an associative link.

Enhanced Entity-Relationship (EER) Diagram for the Workout Tracking System using Chen Notation:
```mermaid
flowchart TD
    %% Entities (Rectangles)
    User[User]
    Prefs[[User_Preferences]]
    Muscle[Muscle Group]
    Ex[Exercise]
    Work[Workout]
    Sess[Workout Session]
    Log[Workout Set Log]

    %% Relationships (Diamonds)
    HasWeak{{Has}}
    Targets{Targets}
    CreatesEx{Creates}
    CreatesWork{Creates}
    Contains{Contains}
    BasedOn{Based On}
    Performs{Performs}
    Logs{Logs}
    Tracks{Tracks}

    %% Specialization (ISA Circles)
    ISA_User((ISA))
    ISA_Ex((ISA))
    ISA_Work((ISA))

    %% Subclasses
    Admin[Admin]
    StdUser[Standard User]
    SysEx[System Exercise]
    CustEx[User Exercise]
    SysWork[System Workout]
    CustWork[User Workout]

    %% Connections with Cardinality (Single lines)
    User --- HasWeak --- Prefs

    User --- ISA_User
    ISA_User --- Admin
    ISA_User --- StdUser

    Ex --- ISA_Ex
    ISA_Ex --- SysEx
    ISA_Ex --- CustEx

    Work --- ISA_Work
    ISA_Work --- SysWork
    ISA_Work --- CustWork

    Muscle ---|1| Targets ---|N| Ex

    User ---|1| CreatesEx ---|N| CustEx
    User ---|1| CreatesWork ---|N| CustWork

    Work ---|M| Contains ---|N| Ex

    User ---|1| Performs ---|N| Sess
    Sess ---|N| BasedOn ---|1| Work

    Sess ---|1| Logs ---|N| Log
    Ex ---|1| Tracks ---|N| Log

    %% Key Attributes (Ovals)
    U_ID((<u>id</u>)) --- User
    C_Sets((target_sets)) --- Contains
    C_Reps((target_reps)) --- Contains
    L_Weight((weight)) --- Log

    %% Styling for Weak Entity
    style HasWeak stroke-width:3px
    style Prefs stroke-width:3px