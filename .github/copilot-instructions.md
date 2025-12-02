# Eisenhower Matrix Web App - Copilot Instructions

## Project Overview
This is a .NET 9 C# 13 ASP.NET Core MVC web application for task management using the Eisenhower Matrix methodology.

## Technology Stack
- .NET 9 / C# 13
- ASP.NET Core MVC
- Entity Framework Core with SQLite
- Bootstrap 5 for responsive UI
- Vanilla JavaScript for interactivity

## Project Structure
- `Models/` - Data models (TaskItem, Settings, Quadrant enum)
- `Data/` - Entity Framework DbContext and migrations
- `Controllers/` - MVC controllers for Tasks, Settings, Export/Import
- `Views/` - Razor views for the UI
- `wwwroot/` - Static files (CSS, JS)
- `Services/` - Business logic services

## Development Guidelines
- Use C# 13 features where appropriate
- Follow async/await patterns for database operations
- Use dependency injection for services
- Keep controllers thin, business logic in services

## Running the Project
```bash
dotnet run
```

## Database
SQLite database stored in `eisenhower.db` in the project root.
