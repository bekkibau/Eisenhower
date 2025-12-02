using Microsoft.EntityFrameworkCore;
using Eisenhower.Data;
using Eisenhower.Models;
using Eisenhower.Models.ViewModels;

namespace Eisenhower.Services;

/// <summary>
/// Service implementation for task operations
/// </summary>
public class TaskService : ITaskService
{
    private readonly EisenhowerDbContext _context;
    private readonly ISettingsService _settingsService;

    public TaskService(EisenhowerDbContext context, ISettingsService settingsService)
    {
        _context = context;
        _settingsService = settingsService;
    }

    public async Task<List<TaskItem>> GetAllTasksAsync()
    {
        return await _context.Tasks
            .OrderBy(t => t.IsCompleted)
            .ThenBy(t => t.DueDate)
            .ThenByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<TaskItem>> GetTasksByQuadrantAsync(Quadrant quadrant)
    {
        return await _context.Tasks
            .Where(t => t.Quadrant == quadrant)
            .OrderBy(t => t.IsCompleted)
            .ThenBy(t => t.DueDate)
            .ThenByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<TaskItem?> GetTaskByIdAsync(int id)
    {
        return await _context.Tasks.FindAsync(id);
    }

    public async Task<TaskItem> CreateTaskAsync(TaskViewModel model)
    {
        var task = new TaskItem
        {
            Title = model.Title,
            Notes = model.Notes,
            Quadrant = model.Quadrant,
            DueDate = model.DueDate,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<TaskItem?> UpdateTaskAsync(int id, TaskViewModel model)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return null;

        task.Title = model.Title;
        task.Notes = model.Notes;
        task.Quadrant = model.Quadrant;
        task.DueDate = model.DueDate;
        task.IsCompleted = model.IsCompleted;
        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return false;

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleCompletionAsync(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return false;

        task.IsCompleted = !task.IsCompleted;
        task.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> ClearCompletedTasksAsync()
    {
        var completedTasks = await _context.Tasks
            .Where(t => t.IsCompleted)
            .ToListAsync();

        _context.Tasks.RemoveRange(completedTasks);
        await _context.SaveChangesAsync();
        return completedTasks.Count;
    }

    public async Task<MatrixViewModel> GetMatrixViewModelAsync(Quadrant? filterQuadrant = null)
    {
        var showCompleted = await _settingsService.GetShowCompletedTasksAsync();
        var viewMode = await _settingsService.GetViewModeAsync();
        var theme = await _settingsService.GetThemeAsync();

        var tasksQuery = _context.Tasks.AsQueryable();

        if (!showCompleted)
        {
            tasksQuery = tasksQuery.Where(t => !t.IsCompleted);
        }

        if (filterQuadrant.HasValue)
        {
            tasksQuery = tasksQuery.Where(t => t.Quadrant == filterQuadrant.Value);
        }

        var tasks = await tasksQuery
            .OrderBy(t => t.IsCompleted)
            .ThenBy(t => t.DueDate)
            .ThenByDescending(t => t.CreatedAt)
            .ToListAsync();

        return new MatrixViewModel
        {
            UrgentImportantTasks = tasks.Where(t => t.Quadrant == Quadrant.UrgentImportant).ToList(),
            NotUrgentImportantTasks = tasks.Where(t => t.Quadrant == Quadrant.NotUrgentImportant).ToList(),
            UrgentNotImportantTasks = tasks.Where(t => t.Quadrant == Quadrant.UrgentNotImportant).ToList(),
            NotUrgentNotImportantTasks = tasks.Where(t => t.Quadrant == Quadrant.NotUrgentNotImportant).ToList(),
            ShowCompletedTasks = showCompleted,
            ViewMode = viewMode,
            Theme = theme,
            FilterQuadrant = filterQuadrant
        };
    }
}
