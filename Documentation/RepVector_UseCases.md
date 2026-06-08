# RepVector Detailed Use Cases

This document provides textual descriptions for the use cases identified in the [Use Case Diagram](./RepVector_UseCaseDiagram.md), providing a deep dive into user interactions and system behavior.

---

## 1. User Management

### UC-1.1: Register Account
*   **Actor:** Standard User
*   **Preconditions:** User is not logged in and does not have an account.
*   **Main Flow:**
    1. Actor navigates to the Registration page.
    2. Actor enters their email, a secure password, and a password confirmation.
    3. System validates the email is unique and the passwords match.
    4. System hashes the password and creates a new User record with the 'User' role.
    5. System creates default User Preferences (Metric units).
*   **Alternate Flow (Email Already Exists):**
    1. At step 3, if the email is already in the database, the System displays an error message.
    2. Actor is prompted to use a different email or go to the Login page.
*   **Postconditions:** Account is created, and the user is redirected to the Login page.

### UC-1.2: Login
*   **Actor:** Standard User
*   **Preconditions:** User has a registered account.
*   **Main Flow:**
    1. Actor enters their email and password on the Login page.
    2. System verifies credentials against the hashed password in the database.
    3. System initializes a server-side session.
*   **Alternate Flow (Invalid Credentials):**
    1. At step 2, if the credentials do not match, the System displays an "Invalid email or password" error.
    2. Actor remains on the Login page to try again.
*   **Postconditions:** User is authenticated and redirected to their Dashboard.

### UC-1.4: Logout
*   **Actor:** Standard User
*   **Preconditions:** User is authenticated.
*   **Main Flow:**
    1. Actor selects the "Logout" option.
    2. System destroys the server-side session and clears authentication cookies.
*   **Postconditions:** User is redirected to the Landing/Login page and can no longer access private data.

### UC-1.5: Manage Preferences
*   **Actor:** Standard User
*   **Preconditions:** User is authenticated.
*   **Main Flow:**
    1. Actor navigates to the Settings/Preferences page.
    2. Actor toggles weight units (KG/LBS) or distance units (KM/Miles).
    3. System updates the `user_preferences` table.
*   **Postconditions:** New unit preferences are applied globally across the UI.

### UC-1.6: Register Administrator
*   **Actor:** Administrator
*   **Preconditions:** The actor has the secret `AdminSecretKey`.
*   **Main Flow:**
    1. Actor navigates to the Admin Registration page.
    2. Actor enters account details and the secret key.
    3. System validates the key and creates an account with the 'Admin' role.
*   **Alternate Flow (Invalid Secret Key):**
    1. At step 3, if the secret key is incorrect, the System displays a "Forbidden" error.
    2. No account is created.
*   **Postconditions:** A new Administrator account is created.

---

## 2. Exercise Catalog

### UC-2.1: View Exercise Library
*   **Actor:** Standard User
*   **Preconditions:** User is authenticated.
*   **Main Flow:**
    1. Actor browses the Exercise index.
    2. System displays a list of "Global" (predefined) exercises and the user's "Private" exercises.
    3. Actor filters by muscle group (e.g., Chest).
*   **Postconditions:** User sees filtered exercise list.

### UC-2.2: Create Private Exercise
*   **Actor:** Standard User
*   **Preconditions:** User is authenticated.
*   **Main Flow:**
    1. Actor clicks "Create New Exercise".
    2. Actor enters name, description, type, and primary muscle group.
    3. System saves the exercise with the user's ID and `is_predefined = 0`.
*   **Postconditions:** The new exercise is available only to the creator.

### UC-2.5: Manage Global Catalog (CRUD)
*   **Actor:** Administrator
*   **Preconditions:** Admin is authenticated.
*   **Main Flow:**
    1. Actor creates, edits, or deletes an exercise.
    2. System marks/keeps these records as `is_predefined = 1`.
*   **Postconditions:** Changes are reflected for all users in the system.

---

## 3. Workout Template Management

### UC-3.1: View All Templates
*   **Actor:** Standard User
*   **Preconditions:** User is authenticated.
*   **Main Flow:**
    1. Actor navigates to the Workouts page.
    2. System retrieves all templates where `user_id` matches the current user OR `is_predefined = 1`.
*   **Postconditions:** User sees their personal and global workout routines.

### UC-3.2: Create Personal Template
*   **Actor:** Standard User
*   **Preconditions:** User is authenticated.
*   **Main Flow:**
    1. Actor enters a name (e.g., "Full Body A") and description.
    2. Actor adds one or more exercises from the catalog.
    3. Actor defines target sets and reps for each exercise.
    4. System saves the workout and links the exercises in the `workout_exercises` table.
*   **Postconditions:** A new reusable template is saved to the user's profile.

---

## 4. Training Sessions

### UC-4.1: Start Training Session
*   **Actor:** Standard User
*   **Preconditions:** User is authenticated; User has selected a workout template.
*   **Main Flow:**
    1. Actor clicks "Start Workout".
    2. System checks for any existing "active" sessions.
    3. If none, system creates a new entry in `workout_sessions` with `status = 'active'`.
*   **Alternate Flow (Active Session Exists):**
    1. At step 2, if an active session is found, the System prompts the Actor to either "Cancel existing and start new" or "Continue current session".
    2. If Actor chooses to cancel, System marks the old session as 'cancelled' and proceeds to create the new one.
*   **Postconditions:** A live timer starts, and the user is redirected to the active workout screen.

### UC-4.2: Log Set Performance
*   **Actor:** Standard User
*   **Preconditions:** User has an "active" training session.
*   **Main Flow:**
    1. Actor performs a set of an exercise.
    2. Actor enters weight, reps, and RPE into the active session interface.
    3. System saves a new record in `workout_set_logs`.
*   **Postconditions:** Performance is saved immediately; the UI updates to show completion of that set.

### UC-4.3: Finish Workout
*   **Actor:** Standard User
*   **Preconditions:** User has an "active" training session.
*   **Main Flow:**
    1. Actor clicks "Finish Workout".
    2. System records the `finished_at` timestamp.
    3. System calculates `total_seconds` and updates the session status to 'completed'.
*   **Postconditions:** Session is closed; user is redirected to the Workout Summary/History.

---

## 5. Progress & History

### UC-5.1: View Session History
*   **Actor:** Standard User
*   **Preconditions:** User is authenticated.
*   **Main Flow:**
    1. Actor navigates to the History tab.
    2. System retrieves all completed sessions for that user, sorted by date.
*   **Postconditions:** User sees a chronological overview of past training.

### UC-5.2: View Session Details
*   **Actor:** Standard User
*   **Preconditions:** User is authenticated; Session belongs to the user.
*   **Main Flow:**
    1. Actor selects a specific past session.
    2. System retrieves all associated `workout_set_logs` and the parent session data.
*   **Postconditions:** User sees exactly what they performed during that specific session.
