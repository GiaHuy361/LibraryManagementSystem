# Library Management System API

A comprehensive RESTful API built with **ASP.NET Core 8**, following **Clean Architecture** principles and **Domain-Driven Design (DDD)**.

## Features
- **Clean Architecture**: Separated into Domain, Application, Infrastructure, and API layers.
- **Entity Framework Core**: Code-first approach with SQL Server.
- **Authentication & Authorization**: Secure JWT-based authentication with Role-based access control (Admin, Librarian, Member).
- **Password Security**: Passwords hashed securely using BCrypt.
- **Design Patterns**: Utilizing Repository Pattern, Unit of Work, Factory Pattern, and Strategy Pattern (for dynamic fine calculation).
- **Graceful Error Handling**: Global exception handling middleware returning standardized RFC-7807 ProblemDetails.
- **Unit Testing**: Comprehensive backend test suite using xUnit and Moq.
- **Auto Data Seeding**: Automatically creates the database and populates dummy data (Books, Authors, Admin user) on startup.

## Technologies Used
- .NET 8 Web API
- Entity Framework Core 9 (SQL Server)
- BCrypt.Net-Next (Password Hashing)
- JWT (JSON Web Tokens)
- xUnit & Moq (Testing)
- Swagger / OpenAPI

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server

### Installation & Run
1. Clone the repository.
2. Open terminal in the root directory.
3. Run the API project:
   ```bash
   dotnet run --project src/LMS.API/LMS.API.csproj
   ```
4. The application will automatically run Entity Framework Migrations, create the database, and seed initial data.
5. Open your browser and navigate to the Swagger UI URL (usually `http://localhost:5000/swagger`).

### Default Admin Credentials
To test the API immediately, use the pre-seeded admin account:
- **Username:** `admin`
- **Password:** `Admin@123`

## Project Structure
- `LMS.Domain` - Core entities and interfaces.
- `LMS.Application` - Business logic, use cases, DTOs, and interface definitions for external services.
- `LMS.Infrastructure` - Database context, EF Core configurations, migrations, and external service implementations (BCrypt, JWT).
- `LMS.API` - Controllers, global error handling, and DI container setup.
- `LMS.Application.Tests` - xUnit test suite.
