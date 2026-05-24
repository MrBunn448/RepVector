# Technical Stack Documentation

This document outlines the technologies chosen for the RepVector project and the rationale behind their selection.

## Core Framework: .NET 10.0
RepVector is built on **.NET 10.0**, the current long-term support (LTS) version of Microsoft's cross-platform development framework.

### Rationale
* **Performance**: .NET 10 provides industry-leading runtime performance, which is critical for handling high-frequency workout data logging.
* **Unified Ecosystem**: Using a single framework for the UI (Razor Pages), API (ASP.NET Core), and Logic layers simplifies development and shared model management.
* **Modern Language Features**: C# 14/15 features are utilized to write concise, safe, and maintainable code.

## Data Storage: MySQL
The application uses **MySQL** as its primary relational database.

### Rationale
* **Reliability**: MySQL is a proven open-source database that handles relational data (Users -> Workouts -> Exercises) efficiently.
* **Performance**: By using `MySqlConnector` for raw SQL execution instead of an ORM, the system achieves maximum throughput and minimal memory overhead.
* **Scalability**: MySQL provides a clear path for horizontal scaling should the user base grow significantly.

## Authentication & Authorization
The system implements a custom authentication flow integrated with ASP.NET Core Sessions.

### Implementation
* **Persistence**: User identities (ID and Role) are stored in the server-side session after successful credential verification.
* **API Security**: The UI layer communicates with the API by passing the `X-User-Id` header. This ensures that the API can verify the requester's identity without maintaining its own session state, adhering to REST principles.
* **Authorization**: Role-Based Access Control (RBAC) is enforced at the Logic layer. Services verify the `UserId` and `UserRole` before allowing any data modification or access to private resources.

### Rationale
* **Simplicity**: For a dedicated workout tracker, session-based authentication provides a robust and easy-to-manage security model without the complexity of JWT or OAuth2.
* **Security**: Storing sensitive user data on the server side (sessions) reduces the risk of client-side tampering.

## Testing Framework: xUnit, Moq & Coverlet
The project maintains a multi-tiered testing strategy to ensure both speed and reliability.

### Tools
* **xUnit**: The primary testing engine used across all test projects.
* **Moq**: Used in **Unit Tests** (`WorkoutTracker.Logic.Tests`) to mock repository interfaces. This allows for testing complex business rules and authorization logic in complete isolation from the database.
* **Coverlet**: A cross-platform code coverage library that tracks how much of the codebase is exercised by the tests.

### Integration Testing
The `WorkoutTracker.IntegrationTests` project provides end-to-end verification. Unlike the unit tests, these tests have a **Zero-Mock Policy**; they use real service and repository implementations connected to the `repvectortest` MySQL database to validate the entire request lifecycle.

### Rationale
* **Fast Unit Tests**: Moq allows us to test thousands of logic permutations in seconds without database overhead.
* **Realistic Integration**: Using a real database for integration tests ensures that raw SQL queries and cascading deletes are functioning as expected.
* **Quality Metrics**: Coverlet provides data-driven insights into test thoroughness.

## Frontend: Razor Pages & Bootstrap 5
The user interface is built using **ASP.NET Core Razor Pages** and styled with **Bootstrap 5**.

### Rationale
* **Developer Productivity**: Razor Pages provides a clean, folder-based routing structure that maps naturally to the application's features (Workouts, Exercises, Sessions).
* **Responsive Design**: Bootstrap 5 ensures the application is fully functional and visually appealing on both mobile devices (used in the gym) and desktop browsers.
