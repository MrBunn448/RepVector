# RepVector Acceptance Tests

This document defines the acceptance tests for the RepVector system, mapped to the Functional Requirements (FR) and Use Cases (UC). 

## About the Format: Gherkin & BDD
These scenarios use the **Gherkin** syntax (Given, When, Then), which is the standard language for **Behavior-Driven Development (BDD)**. 

### Why Gherkin?
*   **Human Readable:** It uses plain English to describe complex behaviors, making it accessible to developers, users, and stakeholders alike.
*   **Living Documentation:** It serves as a single source of truth that stays relevant as the project evolves, ensuring everyone has the same understanding of "done."
*   **Bridge the Gap:** It bridges the communication gap between technical implementation and business requirements.

### Keywords:
*   **Given**: The initial context or precondition.
*   **When**: The specific action or event.
*   **And**: Additional conditions or actions.
*   **Then**: The expected outcome or validation.

---

## 1. Authentication & User Management

### AT-1.1: User Registration (Ref: FR-1.1, UC-1.1)
**Given** I am a new visitor to the RepVector application
**When** I navigate to the Registration page
**And** I enter a unique email, a strong password, and confirm it
**Then** my account should be created successfully
**And** I should be redirected to the Login page or automatically logged in.

### AT-1.2: Secure Login (Ref: FR-1.2, UC-1.2)
**Given** I have a registered account
**When** I enter my correct email and password on the Login page
**Then** I should be granted access to my personal dashboard
**And** the system should recognize my identity for all subsequent actions.

### AT-1.3: Ownership Protection (Ref: FR-1.4)
**Given** I am logged in as User A
**When** I attempt to access a workout template belonging to User B via a direct API call or URL
**Then** the system should deny access with an "Unauthorized" or "Forbidden" error.

---

## 2. Exercise & Muscle Group Catalog

### AT-2.1: View Global Catalog (Ref: FR-2.1, UC-2.1)
**Given** I am any logged-in user
**When** I browse the exercise library
**Then** I should see a list of predefined exercises (e.g., "Bench Press", "Squat")
**And** I should be able to filter them by muscle group.

### AT-2.2: Create Private Exercise (Ref: FR-2.2, UC-2.2)
**Given** I am logged in
**When** I create a custom exercise named "My Secret Movement"
**Then** this exercise should appear in my personal library
**And** it should NOT be visible to other standard users.

### AT-2.3: Admin Global Management (Ref: FR-2.4, UC-2.5)
**Given** I am logged in as an Administrator
**When** I modify a predefined exercise in the global catalog
**Then** the change should be reflected for all users in the system.

---

## 3. Workout Template Management

### AT-3.1: Create Personal Template (Ref: FR-3.1, UC-3.2)
**Given** I am logged in
**When** I create a new workout template named "Monday Upper Body"
**And** I add "Bench Press" and "Pull Ups" to it
**Then** the template should be saved to my profile.

### AT-3.2: Define Exercise Sequence (Ref: FR-3.3)
**Given** I am editing a workout template
**When** I change the order of exercises (e.g., moving "Pull Ups" to be the first exercise)
**Then** the new sequence should be persisted and displayed correctly during a live session.

---

## 4. Live Workout Sessions

### AT-4.1: Start Session from Template (Ref: FR-4.1, UC-4.1)
**Given** I have a template named "Leg Day"
**When** I click "Start Workout" on that template
**Then** a new active session should be initialized
**And** the system should display the first exercise in the routine.

### AT-4.2: Log Set Performance (Ref: FR-4.3, UC-4.2)
**Given** I have an active workout session
**When** I record a set for "Squats" with 100kg for 10 reps at RPE 8
**Then** the data should be saved immediately
**And** the interface should update to show the completed set.

### AT-4.3: Session Conflict Detection (Ref: FR-4.5)
**Given** I already have an active workout session in progress
**When** I attempt to start a different workout template
**Then** the system should prompt me to either cancel the current session or finish it first.

### AT-4.4: Finish Workout (Ref: FR-4.4, UC-4.3)
**Given** I am in an active session
**When** I click "Finish Workout"
**Then** the session status should change to "Completed"
**And** the end time should be recorded
**And** I should be redirected to the session summary.

---

## 5. Progress & History

### AT-5.1: View Training History (Ref: FR-5.1, UC-5.1)
**Given** I have completed several workouts in the past
**When** I navigate to the "History" tab
**Then** I should see a list of my past sessions ordered by date (newest first).

### AT-5.2: View Session Details (Ref: FR-5.2, UC-5.2)
**Given** I am looking at my history
**When** I select a specific session from last Tuesday
**Then** I should see every set, rep, and weight I logged during that specific workout.

---

## 6. User Customization

### AT-6.1: Unit Preference Toggle (Ref: FR-6.1, UC-1.4)
**Given** my current preference is set to "Metric (KG)"
**When** I change my settings to "Imperial (LBS)"
**Then** all weights displayed in the application (templates, history, active sessions) should be converted or displayed in LBS.

---

# Test Execution Report

This section is for testers to record the results of the acceptance tests.

**Date of Execution:** [YYYY-MM-DD]  
**Tester Name:** [Name]  
**Environment:** [Local / Production]  
**Overall Result:** [PASS / FAIL / PARTIAL]

## 1. Summary Table
| Metric | Count |
| :--- | :--- |
| **Total Test Cases** | 16 |
| **Passed** | |
| **Failed** | |
| **Skipped** | |

## 2. Detailed Findings
| ID | Test Case Name | Status (P/F) | Observation / Bug Notes |
| :--- | :--- | :--- | :--- |
| **AT-1.1** | User Registration | | |
| **AT-1.2** | Secure Login | | |
| **AT-1.3** | Ownership Protection | | |
| **AT-2.1** | View Global Catalog | | |
| **AT-2.2** | Create Private Exercise | | |
| **AT-2.3** | Admin Global Management | | |
| **AT-3.1** | Create Personal Template | | |
| **AT-3.2** | Define Exercise Sequence | | |
| **AT-4.1** | Start Session from Template | | |
| **AT-4.2** | Log Set Performance | | |
| **AT-4.3** | Session Conflict Detection | | |
| **AT-4.4** | Finish Workout | | |
| **AT-5.1** | View Training History | | |
| **AT-5.2** | View Session Details | | |
| **AT-6.1** | Unit Preference Toggle | | |

