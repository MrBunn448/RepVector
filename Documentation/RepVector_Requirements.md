# System Requirements (Full Scope - MoSCoW)

This document provides a comprehensive list of all functional and non-functional requirements for the RepVector system as of May 2026.

## 1. Functional Requirements (FR)

### 1.1 Authentication & User Management
| ID | Requirement | Priority | Description |
| :--- | :--- | :--- | :--- |
| **FR-1.1** | **User Registration** | **MUST** | New users can create accounts with unique emails and hashed passwords. |
| **FR-1.2** | **Secure Login** | **MUST** | Existing users can authenticate safely to access private data. |
| **FR-1.3** | **Admin Role** | **MUST** | System identifies "Admin" users to allow management of global templates/exercises. |
| **FR-1.4** | **Ownership Protection** | **MUST** | Users can only view, edit, or delete their own data (verified via `IAuthorizationService`). |

### 1.2 Exercise & Muscle Group Catalog
| ID | Requirement | Priority | Description |
| :--- | :--- | :--- | :--- |
| **FR-2.1** | **Global Catalog** | **MUST** | Access a library of "Predefined" exercises available to everyone. |
| **FR-2.2** | **Custom Exercises** | **MUST** | Users can create personal exercises that are hidden from others. |
| **FR-2.3** | **Muscle Group Mapping** | **MUST** | Exercises are categorized by muscle groups (Chest, Back, etc.) for easier browsing. |
| **FR-2.4** | **Admin Override** | **MUST** | Only Admins can modify or delete "Predefined" exercises. |

### 1.3 Workout Template Management
| ID | Requirement | Priority | Description |
| :--- | :--- | :--- | :--- |
| **FR-3.1** | **Template CRUD** | **MUST** | Create, Read, Update, and Delete workout templates. |
| **FR-3.2** | **Exercise Linkage** | **MUST** | Add specific exercises to a template with target sets, reps, and RPE. |
| **FR-3.3** | **Sort Order** | **MUST** | Define and update the sequence of exercises within a workout. |
| **FR-3.4** | **Predefined Templates**| **MUST** | Admins can create "Global" templates for all users to follow. |

### 1.4 Live Workout Sessions
| ID | Requirement | Priority | Description |
| :--- | :--- | :--- | :--- |
| **FR-4.1** | **Session Initiation** | **MUST** | Start a live session based on any accessible template. |
| **FR-4.2** | **Active Tracking** | **MUST** | Maintain state for a single "Active" session per user. |
| **FR-4.3** | **Performance Logging** | **MUST** | Record actual weight, reps, and RPE for every set performed. |
| **FR-4.4** | **Session Completion** | **MUST** | Mark a session as "Completed" to stop the timer and save history. |
| **FR-4.5** | **Conflict Detection** | **MUST** | Prevent starting a new session if one is already active without confirmation. |

### 1.5 Progress & History
| ID | Requirement | Priority | Description |
| :--- | :--- | :--- | :--- |
| **FR-5.1** | **Training History** | **MUST** | View a chronological list of all past workout sessions. |
| **FR-5.2** | **Log Details** | **MUST** | View specific set-by-set performance data for any historical session. |
| **FR-5.3** | **History Management** | **SHOULD** | Delete old or accidental session logs. |
| **FR-5.4** | **Volume Tracking** | **COULD** | Automatically calculate total tonnage lifted (handled by future analytics). |

### 1.6 User Customization
| ID | Requirement | Priority | Description |
| :--- | :--- | :--- | :--- |
| **FR-6.1** | **Unit Preference** | **MUST** | Toggle between Metric (KG) and Imperial (LBS) units. |
| **FR-6.2** | **Profile Settings** | **SHOULD** | Manage account details and display preferences. |

---

## 2. Non-Functional Requirements (NFR)

| ID | Requirement | Priority | Description |
| :--- | :--- | :--- | :--- |
| **NFR-1** | **Raw SQL Execution** | **MUST** | Absolute ban on ORMs (EF Core) to ensure maximum data access speed. |
| **NFR-2** | **N-Tier Separation** | **MUST** | Clear separation between UI, API, Logic, and DAL projects. |
| **NFR-3** | **Stateless API** | **MUST** | The backend API must be stateless, using headers (X-User-Id) for identity. |
| **NFR-4** | **Responsiveness** | **SHOULD** | UI must scale from desktop to mobile (Gym environment). |
| **NFR-5** | **Error Transparency** | **SHOULD** | Use `Result` objects to provide clear failure reasons instead of generic errors. |
