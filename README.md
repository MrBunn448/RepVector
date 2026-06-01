# RepVector

RepVector is a modern, free workout tracking system built with .NET 10. It empowers users to take control of their fitness journey through precise routine management, performance logging, and structured progress analysis.

## Core Features

*   **Intelligent Workout Builder:** Create reusable templates with target sets, reps, and RPE.
*   **Global Exercise Library:** Access a master catalog of exercises categorized by muscle groups and equipment types.
*   **Live Session Tracking:** Log your workouts in real-time with an intuitive active-session interface.
*   **Performance Analytics:** Historical data preservation for every set, including weight, reps, and effort (RPE).
*   **Admin Tools:** Dedicated administrative role for managing global content and system-wide templates.

## System Architecture

RepVector follows a **Decoupled 4-Layer Architecture**, ensuring that business logic remains independent of infrastructure and UI concerns.

*   **UI Layer (Razor Pages):** Clean, responsive interface for daily use.
*   **API Layer (ASP.NET Core):** Secure RESTful gateway with role-based authorization.
*   **Logic Layer (Core Business):** Pure domain logic, validation, and service orchestration.
*   **DAL Layer (MySQL):** High-speed data access using optimized raw SQL queries.

For more details, see the [Architecture Documentation](Documentation/RepVector_Architecture.md).

## Getting Started

### 1. Database Setup
Ensure you have a MySQL instance running.
1.  **Schema Initialization:** Run `Database/RepVector_Blueprint.sql` to create the tables.
2.  **Seed Data:** Run `Database/RepVector_Seeder.sql` to populate initial muscle groups, exercises, and test accounts.

### 2. Configuration
Update your `appsettings.json` in `WorkoutTracker.Api` with your database credentials. 

> [!IMPORTANT]
> **Security Best Practice:** Never commit real passwords to GitHub. For production, use **Environment Variables**.

#### Environment Variables (Production)
In a production environment (like Azure, Docker, or a VPS), set the following environment variables to override the `appsettings.json` values securely:

| Variable Name | Description | Example Value |
| :--- | :--- | :--- |
| `ConnectionStrings__DefaultConnection` | Live Database String | `Server=...;Database=...;Uid=...;Pwd=...;` |
| `AdminSecretKey` | Secret key for Admin registration | `YourSuperSecretKey` |

#### Local Development (.env)
1. Copy `.env.example` to a new file named `.env`.
2. Fill in your local or live credentials.
3. The `.env` file is already included in `.gitignore` to prevent accidental commits.

### 3. Admin Registration
To register a new administrator account through the UI, you must provide the system's **Admin Secret Key**.
*   **Secret Key:** `RepVector2026`
*   **Configuration:** This can be changed in the `appsettings.json` of the API project under the `AdminSecretKey` property.

## Testing & Credentials

The following accounts are provided by the seeder script for easy testing and evaluation.

| Role  | Email | Password |
| :--- | :--- | :--- |
| **Admin** | `admin@repvector.com` | `Password123!` |
| **User** | `user1@repvector.com` | `Password123!` |
| **User** | `user2@repvector.com` | `Password123!` |
| **User** | `user3@repvector.com` | `Password123!` |
| **User** | `user4@repvector.com` | `Password123!` |
| **User** | `user5@repvector.com` | `Password123!` |

> **Note:** For security, it is recommended to register your own accounts and delete these seed accounts before any production deployment.

## Technical Stack

*   **Backend:** .NET 10 (C#)
*   **Database:** MySQL (Raw SQL / MySqlConnector)
*   **Frontend:** ASP.NET Core Razor Pages, Bootstrap 5, JavaScript
*   **Modeling:** Mermaid.js for documentation diagrams

---
*Developed as an Individual Project for Semester 2.*
