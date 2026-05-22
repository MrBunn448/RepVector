# Workout Tracker

A layered .NET 10 web application for managing workouts and exercises, built using a custom architecture with a Razor Pages frontend, ASP.NET Core Web API backend, and a MySQL database.

---

## Overview

This application allows users to:

- View a list of workouts
- View workout details
- View exercises linked to each workout

The system is built with a strict separation of concerns using a layered architecture and does not use Entity Framework.



## Architecture

The application follows a **4-layer architecture**:

```
UI (Razor Pages)
↓
API (Controllers)
↓
Logic (Services)
↓
DAL (Repositories)
↓
Database (MySQL)

```

---

## Layers Explained

### UI Layer (WorkoutTracker.UI)
- Built with Razor Pages
- Displays workouts and exercise data
- Communicates with the API via HTTP clients
- Contains no business logic or SQL access

---

### API Layer (WorkoutTracker.Api)
- Exposes REST endpoints
- Acts as a bridge between UI and backend logic
- Calls service layer only

---

### Logic Layer (WorkoutTracker.Logic)
- Contains business logic and orchestration
- Coordinates data between API and DAL
- Does not directly interact with UI or database

---

### DAL (WorkoutTracker.DAL)
- Handles all database operations
- Executes raw SQL using MySQL connector
- Maps query results to models

---

### Models (WorkoutTracker.Models)
- Contains shared domain models:
  - Workout
  - Exercise
- Used across all layers

---

## Database

MySQL database with the following structure (v1):

### Workout
- Id (PK)
- Name (unique, varchar 20)
- Description (varchar 50)

### Exercise
- Id (PK)
- Name (unique, varchar 20)
- Sets (int)
- Reps (int)
- WorkoutId (FK → Workout)

---

## Key Design Principles

- No Entity Framework – all data access is done via raw SQL
- Dependency Injection – used for all service and repository wiring
- Separation of Concerns – each layer has a single responsibility
- Async-first design for I/O operations
- Loose coupling between layers

---

## Technologies

- .NET 10
- ASP.NET Core Web API
- Razor Pages
- MySQL (phpMyAdmin)
- MySqlConnector
- Dependency Injection (built-in ASP.NET Core DI)

---

## Data Flow

When a user opens a workout detail page:

1. UI sends request to API
2. API calls Service layer
3. Service calls Repository (DAL)
4. Repository queries MySQL
5. Data is mapped to models
6. Response flows back up to UI


---

## Current Features

- View all workouts
- View workout details
- View exercises per workout

---

## Planned Features

- Add/edit/delete workouts
- Add/edit/delete exercises
- Workout planning (assign exercises dynamically)
- Authentication system (Login/Registration)
- User-specific workout plans

---

## Security Notes

- Connection strings are stored in appsettings.json
- Sensitive configuration is excluded from Git using .gitignore
- No credentials are hardcoded in source code

---

## Development Principles

This project is built to follow:

- SOLID principles (Partly, DTO's and Interfaces will be added in future iterations)
- Clean Architecture concepts (simplified version)
- Explicit dependency management
- Testable service structure

---

## Notes

This is a learning-focused project designed to understand:
- layered architecture
- dependency injection
- API-driven design
- raw SQL data access patterns
```

```
```
