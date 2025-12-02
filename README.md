# Eisenhower Matrix Web App

A task management web application built with ASP.NET Core 9 and C# 13, implementing the Eisenhower Matrix methodology for prioritizing tasks based on urgency and importance.

![.NET 9](https://img.shields.io/badge/.NET-9.0-purple)
![C# 13](https://img.shields.io/badge/C%23-13-blue)
![SQLite](https://img.shields.io/badge/SQLite-3-green)
![Bootstrap 5](https://img.shields.io/badge/Bootstrap-5.3-purple)

## Features

### Task Management
- ✅ Create, edit, and delete tasks
- ✅ Assign tasks to one of four quadrants
- ✅ Mark tasks as complete/incomplete
- ✅ Add due dates with overdue indicators
- ✅ Add notes to tasks

### Eisenhower Matrix Quadrants
- **Q1 - Do First**: Urgent and Important tasks
- **Q2 - Schedule**: Not Urgent but Important tasks
- **Q3 - Delegate**: Urgent but Not Important tasks
- **Q4 - Eliminate**: Not Urgent and Not Important tasks

### View Options
- 📊 **Matrix View**: Classic 2x2 grid layout
- 📋 **List View**: Vertical list by quadrant
- 🔍 **Filter**: View tasks by specific quadrant
- 👁️ **Toggle Completed**: Show/hide completed tasks

### Settings & Customization
- 🌓 **Theme**: Light, Dark, or System preference
- 📤 **Export**: Download tasks as JSON backup
- 📥 **Import**: Restore tasks from backup
- 🧹 **Clear Completed**: Bulk delete completed tasks

### Technical Features
- Responsive design (works on desktop, tablet, mobile)
- SQLite database for simple deployment
- Keyboard shortcuts (Ctrl+N for new task)
- Auto-dismissing notifications

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

## Installation

### Option 1: Download macOS App (Recommended)

Download the latest release from the [Releases](../../releases) page:

- **Apple Silicon (M1/M2/M3)**: `Eisenhower-x.x.x-osx-arm64.dmg`
- **Intel Mac**: `Eisenhower-x.x.x-osx-x64.dmg`

**Installation steps:**
1. Open the downloaded DMG file
2. Drag the Eisenhower app to your Applications folder
3. **First launch only**: Right-click the app and select "Open" (required for unsigned apps). You may need to Open System Settings. Click Privacy & Security, scroll down, and click the Open Anyway button to confirm your intent to open or install the app. See [Apple Support steps](https://support.apple.com/en-us/102445#:~:text=Click%20Privacy%20%26%20Security%2C%20scroll,open%20or%20install%20the%20app).

The app will automatically open in your default browser. Your data is stored locally.

### Option 2: Run from Source

```bash
# Restore dependencies
dotnet restore

# Run the application
dotnet run
```

The application will start at `http://localhost:5086`.

## Getting Started

### Run

```bash
# Clone
git clone https://github.com/bekkibau/Eisenhower.git

# Restore dependencies
dotnet restore

# Run the application
dotnet run
```

The application will start at `https://localhost:5086` (or `http://localhost:5086`).

### Database

The application uses SQLite with Entity Framework Core. The database file (`eisenhower.db`) is automatically created on first run in the project directory.

## Project Structure

```
Eisenhower/
├── Controllers/           # MVC Controllers
│   ├── HomeController.cs      # Main matrix view
│   ├── TasksController.cs     # Task CRUD operations
│   ├── SettingsController.cs  # Settings management
│   └── ExportController.cs    # Export/Import functionality
├── Data/
│   └── EisenhowerDbContext.cs # Entity Framework DbContext
├── Models/
│   ├── TaskItem.cs           # Task entity
│   ├── Settings.cs           # Settings entity
│   ├── Quadrant.cs           # Quadrant enum
│   ├── ExportData.cs         # Export/Import DTOs
│   └── ViewModels/           # View models
├── Services/                 # Business logic services
│   ├── ITaskService.cs
│   ├── TaskService.cs
│   ├── ISettingsService.cs
│   ├── SettingsService.cs
│   ├── IExportImportService.cs
│   └── ExportImportService.cs
├── Views/                    # Razor views
│   ├── Home/
│   ├── Tasks/
│   ├── Settings/
│   ├── Export/
│   └── Shared/
├── wwwroot/
│   ├── css/site.css         # Custom styles
│   └── js/site.js           # Client-side JavaScript
└── Program.cs               # Application entry point
```

## Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| `Ctrl/Cmd + N` | Create new task |
| `Escape` | Return to home |

## API Endpoints

### Tasks
- `GET /` - View matrix
- `GET /Tasks/Create` - Create task form
- `POST /Tasks/Create` - Create task
- `GET /Tasks/Edit/{id}` - Edit task form
- `POST /Tasks/Edit/{id}` - Update task
- `POST /Tasks/Delete/{id}` - Delete task
- `POST /Tasks/Toggle/{id}` - Toggle task completion

### Settings
- `GET /Settings` - Settings page
- `POST /Settings/SetTheme` - Change theme
- `POST /Settings/SetViewMode` - Change view mode
- `POST /Settings/SetShowCompleted` - Toggle completed visibility
- `POST /Settings/ClearCompleted` - Delete all completed tasks

### Export/Import
- `GET /Export/Download` - Download JSON backup
- `GET /Export/Import` - Import form
- `POST /Export/Import` - Import JSON backup

## Technologies Used

- **Backend**: ASP.NET Core 9, C# 13
- **Database**: SQLite with Entity Framework Core 9
- **Frontend**: Bootstrap 5.3, Bootstrap Icons
- **Architecture**: MVC with Service Layer pattern

## Development

### Running in Development Mode

```bash
dotnet watch run
```

This enables hot reload for faster development.

### Adding Migrations (if needed)

```bash
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

## License

MIT License - feel free to use this project for personal or commercial purposes.

## About the Eisenhower Matrix

The Eisenhower Matrix, also known as the Urgent-Important Matrix, is a time management tool attributed to Dwight D. Eisenhower. It helps prioritize tasks by categorizing them into four quadrants based on urgency and importance:

1. **Do First** (Urgent + Important): Crisis, deadlines, problems
2. **Schedule** (Not Urgent + Important): Planning, prevention, improvement
3. **Delegate** (Urgent + Not Important): Interruptions, some meetings
4. **Eliminate** (Not Urgent + Not Important): Time wasters, pleasant activities

The key insight is to focus more time on Quadrant 2 (important but not urgent) to prevent tasks from becoming urgent crises.
