# System Requirements (MoSCoW)

This document outlines the functional and non-functional requirements for the RepVector workout tracking system, prioritized using the MoSCoW method to align with the current implementation.

## 1. Functional Requirements (FR)

| ID | Requirement | Priority | Description |
| :--- | :--- | :--- | :--- |
| **FR-1** | **Secure Authentication** | **MUST** | Registration and Login with secure password hashing via `AuthService`. |
| **FR-2** | **Role-Based Access Control** | **MUST** | Distinguish between standard Users and Admins (implemented in `IAuthorizationService`). |
| **FR-3** | **Exercise Catalog** | **MUST** | CRUD for exercises categorized by muscle groups. |
| **FR-4** | **Workout Templates** | **MUST** | Create reusable plans with target sets and reps (`WorkoutService`). |
| **FR-5** | **Live Performance Logging** | **MUST** | Record real-time weight, reps, and RPE during a session (`WorkoutSessionService`). |
| **FR-6** | **User Preferences** | **SHOULD** | Personal settings like weight units (KG/LBS) via `PreferenceService`. |
| **FR-7** | **Conflict Handling** | **SHOULD** | Detect and manage overlapping active sessions. |
| **FR-8** | **Session History** | **SHOULD** | Review and delete past workout logs. |
| **FR-9** | **Live Routine Adjustment** | **COULD** | Dynamically add/remove exercises during a workout session. |
| **FR-10** | **Social Features** | **WON'T** | Sharing workouts or following other users (out of scope). |

## 2. Non-Functional Requirements (NFR)

| ID | Requirement | Priority | Description |
| :--- | :--- | :--- | :--- |
| **NFR-1** | **High Performance** | **MUST** | Direct SQL execution (MySqlConnector); **No ORM** allowed. |
| **NFR-2** | **System Modularity** | **MUST** | Isolation of business logic from UI and DAL (N-Tier Architecture). |
| **NFR-3** | **Data Integrity** | **MUST** | Strict enforcement of foreign key constraints in the database schema. |
| **NFR-4** | **API Decoupling** | **MUST** | Frontend must interact with the backend exclusively via REST API. |
| **NFR-5** | **Responsiveness** | **SHOULD** | Layout optimized for mobile devices in a gym environment. |
| **NFR-6** | **Visual Feedback** | **SHOULD** | Immediate confirmation of successful data entry (e.g., set logs). |
