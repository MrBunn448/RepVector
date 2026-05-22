# Use Case Documentation

This document describes the primary use cases for the RepVector system, detailing the interactions between the User (Actor) and the application.

## Use Case: Log Workout Performance (UC-4.2)

**Actor:** Standard User

### Description
The user records the actual weight, repetitions, and intensity (RPE) for a specific set during an active workout session.

### Preconditions
1. The user is authenticated.
2. An active workout session is currently running for the user.
3. The user is on the "Active Workout" page.

### Postconditions
1. A performance log record is created and linked to the active session.
2. The user's progress is updated in real-time.
3. The input UI is reset for the next set.

### Main Success Scenario
1. **User** identifies the exercise they just performed.
2. **User** enters the **Weight** used (e.g., 80 kg).
3. **User** enters the **Reps** completed (e.g., 10).
4. **User** (Optional) selects an **RPE** value (1-10).
5. **User** clicks the **"Log Set"** button.
6. **System** validates that the session is still active and the inputs are valid.
7. **System** saves the set log via the API (`WorkoutSessionService`).
8. **System** displays the logged set in the exercise history list.
9. **System** provides a brief visual confirmation (e.g., a green check or flash).

### Extensions (Alternative Flows)

*   **E1: Session Not Active**
    *   6a. System detects the session has been closed or cancelled.
    *   6b. System displays an error: "Session is no longer active."
    *   6c. System redirects user to the Workout History page.

*   **E2: Invalid Input**
    *   6a. System detects non-numeric or negative values.
    *   6b. System highlights the invalid fields and prevents submission.

*   **E3: Network Error**
    *   7a. System fails to reach the API.
    *   7b. System displays a retry message: "Failed to save. Please check your connection."

---

## Use Case: Create Workout Template (UC-3.2)

**Actor:** Standard User / Admin

### Description
The user creates a reusable workout plan by selecting exercises and setting target goals.

### Preconditions
1. The user is authenticated.

### Postconditions
1. A new workout template is saved and available for starting sessions.

### Main Success Scenario
1. **User** navigates to "Create Workout".
2. **User** enters a **Name** and **Description**.
3. **User** selects exercises from the global catalog.
4. **User** defines target sets, reps, and RPE for each selected exercise.
5. **User** clicks **"Save Template"**.
6. **System** validates ownership and permissions (Admins can set "Predefined").
7. **System** persists the template and the exercise links.
8. **System** redirects the user to the Templates Index.
