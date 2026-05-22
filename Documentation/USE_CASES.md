# Use Case Documentation

This document maps the project functionality into discrete Use Cases, describing the user intent and the system's role.

## User Management

| Use Case ID | Name | Description |
|---|---|---|
| UC-1.1 | **User Registration** | A new user creates an account to track their workouts. |
| UC-1.2 | **User Login** | A user authenticates to access their private data. |
| UC-1.3 | **Manage Preferences** | A user sets personal preferences (e.g., metric vs imperial). |

## Exercise Management

| Use Case ID | Name | Description |
|---|---|---|
| UC-2.1 | **View Exercise Catalog** | User browses both predefined (system) exercises and personal ones. |
| UC-2.2 | **Create Custom Exercise** | User adds a new exercise to their personal catalog. |
| UC-2.3 | **Edit/Delete Exercise** | User modifies or removes an exercise they created. |
| UC-2.4 | **Manage Muscle Groups** | System provides a list of muscle groups to categorize exercises. |

## Workout Template Management

| Use Case ID | Name | Description |
|---|---|---|
| UC-3.1 | **View Workout Templates** | User views a list of their templates and global system templates. |
| UC-3.2 | **Create Workout Template** | User creates a reusable workout plan by selecting exercises and setting targets (reps/sets). |
| UC-3.3 | **Edit/Update Template** | User modifies an existing template (e.g., changing sort order of exercises). |
| UC-3.4 | **Delete Template** | User removes a template they created. |

## Workout Execution (Sessions)

| Use Case ID | Name | Description |
|---|---|---|
| UC-4.1 | **Start Workout Session** | User starts a live session based on a template. |
| UC-4.2 | **Log Performance** | User records actual weight and reps for each set during the session. |
| UC-4.3 | **Complete Session** | User finishes the session, and it is moved to historical logs. |
| UC-4.4 | **View Session History** | User reviews past performances to track progress. |

## Administrative Actions

| Use Case ID | Name | Description |
|---|---|---|
| UC-5.1 | **Create Global Template** | Admin creates a predefined workout template available to all users. |
| UC-5.2 | **Manage Global Exercises** | Admin adds or modifies exercises in the system's master catalog. |
