# System Requirements

This document outlines the functional and non-functional requirements for the RepVector workout tracking system, incorporating specific qualities and constraints for each requirement where applicable.

## 1. Functional Requirements

Functional requirements define the core behaviors and features of the system.

### 1.1 User Management
- **FR-1.1: Registration and Login**
    - **Quality**: The system can register and authenticate users to protect private fitness data.
    - **Constraint**: The system must not store passwords in plain-text; secure hashing is mandatory.
- **FR-1.2: Role-Based Access Control**
    - **Quality**: The system can distinguish between standard Users and Admins, providing tailored management capabilities.
    - **Constraint**: The system must not allow standard users to perform administrative actions, such as creating global templates.
- **FR-1.3: User Preferences**
    - **Quality**: The system can store personal settings like weight units (KG/LBS) to customize the user experience.

### 1.2 Exercise Catalog
- **FR-1.2: Catalog Management**
    - **Quality**: The system can manage a vast library of exercises categorized by muscle groups.
    - **Constraint**: Standard users must not be able to modify "RepVector Official" exercises.
- **FR-1.3: Custom Exercises**
    - **Quality**: The system can allow users to create and manage their own private exercise movements.
    - **Constraint**: The system must not expose personal exercises to other users.

### 1.3 Workout Templates
- **FR-1.3: Template Creation**
    - **Quality**: The system can bundle exercises into reusable workout plans with target sets and reps.
    - **Constraint**: The system must not delete templates that are currently linked to an active workout session.

### 1.4 Workout Sessions (Execution)
- **FR-1.4: Live Performance Logging**
    - **Quality**: The system can record real-time data (weight, reps, RPE) during a training session.
    - **Constraint**: The system must not allow logging data for a session that has already been marked as "Completed".
- **FR-1.5: Dynamic Routine Adjustment**
    - **Quality**: The system can dynamically add/remove exercises or sets while a workout is in progress.
- **FR-1.6: Session Conflict Handling**
    - **Quality**: The system can detect and manage overlapping sessions.
    - **Constraint**: The system must not start a new session if one is already active without explicit user confirmation to cancel the old one.

---

## 2. Non-Functional Requirements

Non-functional requirements specify the quality attributes and technical constraints of the system.

### 2.1 Performance and Reliability
- **NFR-2.1: Data Access Speed**
    - **Quality**: The system can provide near-instantaneous responses for logging and retrieval.
    - **Constraint**: The system must not use an ORM (like Entity Framework); direct SQL execution is required to maintain performance.
- **NFR-2.2: Data Integrity**
    - **Quality**: The system can ensure consistent data across all tables.
    - **Constraint**: The system must not allow orphan records; foreign key constraints must be strictly enforced.

### 2.2 Architecture and Maintainability
- **NFR-2.3: System Modularity**
    - **Quality**: The system can isolate business rules from presentation and data access details.
    - **Constraint**: The presentation layer must not communicate directly with the database.
- **NFR-2.4: Client Extensibility**
    - **Quality**: The system can support multiple frontend types (Web, Mobile) via its decoupled API.
- **NFR-2.5: Automated Verification**
    - **Quality**: The system can be validated automatically using unit and integration tests.

### 2.3 Usability and Design
- **NFR-2.6: Mobile Responsiveness**
    - **Quality**: The system can adapt its layout for use on smartphones in a gym environment.
- **NFR-2.7: Visual Feedback**
    - **Quality**: The system can provide immediate visual confirmation of successful data entry.
