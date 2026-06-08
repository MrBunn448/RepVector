# System Requirements (Full Scope - MoSCoW)

This document provides a comprehensive list of all functional and non-functional requirements for the RepVector June 2026

## 1. Functional Requirements (FR)

### 1.1 Authentication & User Management
| ID | Requirement | Priority | Status | Description |
| :--- | :--- | :--- | :--- | :--- |
| **FR-1.1** | **User Registration** | **MUST** | **v** | New users can create accounts with unique emails and hashed passwords. |
| **FR-1.2** | **Secure Login** | **MUST** | **v** | Existing users can authenticate safely to access private data. |
| **FR-1.3** | **Admin Role** | **MUST** | **v** | System identifies "Admin" users to allow management of global templates/exercises. |
| **FR-1.4** | **Ownership Protection** | **MUST** | **v** | Users can only view, edit, or delete their own data (verified via `IAuthorizationService`). |

### 1.2 Exercise & Muscle Group Catalog
| ID | Requirement | Priority | Status | Description |
| :--- | :--- | :--- | :--- | :--- |
| **FR-2.1** | **Global Catalog** | **MUST** | **v** | Access a library of "Predefined" exercises available to everyone. |
| **FR-2.2** | **Custom Exercises** | **MUST** | **v** | Users can create personal exercises that are hidden from others. |
| **FR-2.3** | **Muscle Group Mapping** | **MUST** | **v** | Exercises are categorized by muscle groups (Chest, Back, etc.) for easier browsing. |
| **FR-2.4** | **Admin Override** | **MUST** | **v** | Only Admins can modify or delete "Predefined" exercises. |

### 1.3 Workout Template Management
| ID | Requirement | Priority | Status | Description |
| :--- | :--- | :--- | :--- | :--- |
| **FR-3.1** | **Template CRUD** | **MUST** | **v** | Create, Read, Update, and Delete workout templates. |
| **FR-3.2** | **Exercise Linkage** | **MUST** | **v** | Add specific exercises to a template with target sets, reps, and RPE. |
| **FR-3.3** | **Sort Order** | **MUST** | **v** | Define and update the sequence of exercises within a workout. |
| **FR-3.4** | **Predefined Templates**| **MUST** | **v** | Admins can create "Global" templates for all users to follow. |

### 1.4 Live Workout Sessions
| ID | Requirement | Priority | Status | Description |
| :--- | :--- | :--- | :--- | :--- |
| **FR-4.1** | **Session Initiation** | **MUST** | **v** | Start a live session based on any accessible template. |
| **FR-4.2** | **Active Tracking** | **MUST** | **v** | Maintain state for a single "Active" session per user. |
| **FR-4.3** | **Performance Logging** | **MUST** | **v** | Record actual weight, reps, and RPE for every set performed. |
| **FR-4.4** | **Session Completion** | **MUST** | **v** | Mark a session as "Completed" to stop the timer and save history. |
| **FR-4.5** | **Conflict Detection** | **MUST** | **v** | Prevent starting a new session if one is already active without confirmation. |

### 1.5 Progress & History
| ID | Requirement | Priority | Status | Description |
| :--- | :--- | :--- | :--- | :--- |
| **FR-5.1** | **Training History** | **MUST** | **v** | View a chronological list of all past workout sessions. |
| **FR-5.2** | **Log Details** | **MUST** | **v** | View specific set-by-set performance data for any historical session. |
| **FR-5.3** | **History Management** | **SHOULD** | **v** | Delete old or accidental session logs. |
| **FR-5.4** | **Volume Tracking** | **WON'T** | **X** | Automatically calculate total tonnage lifted (handled by future analytics). |

### 1.6 User Customization
| ID | Requirement | Priority | Status | Description |
| :--- | :--- | :--- | :--- | :--- |
| **FR-6.1** | **Unit Preference** | **MUST** | **v** | Toggle between Metric (KG) and Imperial (LBS) units. |
| **FR-6.2** | **Profile Settings** | **SHOULD** | **v** | Manage account details and display preferences. |

---

## 2. Non-Functional Requirements (NFR)

| ID | Requirement | Priority | Status | Description |
| :--- | :--- | :--- | :--- | :--- |
| **NFR-1** | **Raw SQL Execution** | **MUST** | **v** | Absolute ban on ORMs (EF Core) to ensure maximum data access speed. |
| **NFR-2** | **N-Tier Separation** | **MUST** | **v** | Clear separation between UI, API, Logic, and DAL projects. |
| **NFR-3** | **Stateless API** | **MUST** | **v** | The backend API must be stateless, using headers (X-User-Id) for identity. |
| **NFR-4** | **Responsiveness** | **SHOULD** | **v** | UI must scale from desktop to mobile (Gym environment). |
| **NFR-5** | **Error Transparency** | **SHOULD** | **v** | Use `Result` objects to provide clear failure reasons instead of generic errors. |
