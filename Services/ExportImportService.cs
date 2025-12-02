using Microsoft.EntityFrameworkCore;
using Eisenhower.Data;
using Eisenhower.Models;

namespace Eisenhower.Services;

/// <summary>
/// Service implementation for export/import operations
/// </summary>
public class ExportImportService : IExportImportService
{
    private readonly EisenhowerDbContext _context;
    private readonly ISettingsService _settingsService;

    public ExportImportService(EisenhowerDbContext context, ISettingsService settingsService)
    {
        _context = context;
        _settingsService = settingsService;
    }

    public async Task<ExportData> ExportDataAsync()
    {
        var tasks = await _context.Tasks.ToListAsync();
        var settings = await _settingsService.GetAllSettingsAsDictionaryAsync();

        return new ExportData
        {
            Version = "1.0",
            ExportedAt = DateTime.UtcNow,
            Tasks = tasks.Select(t => new TaskExportItem
            {
                Title = t.Title,
                Notes = t.Notes,
                Quadrant = (int)t.Quadrant,
                DueDate = t.DueDate,
                IsCompleted = t.IsCompleted,
                CreatedAt = t.CreatedAt
            }).ToList(),
            Settings = settings
        };
    }

    public async Task<(int tasksImported, bool settingsImported)> ImportDataAsync(ExportData data, bool clearExisting = false)
    {
        if (clearExisting)
        {
            _context.Tasks.RemoveRange(_context.Tasks);
            await _context.SaveChangesAsync();
        }

        var tasksImported = 0;
        foreach (var taskData in data.Tasks)
        {
            var task = new TaskItem
            {
                Title = taskData.Title,
                Notes = taskData.Notes,
                Quadrant = (Quadrant)taskData.Quadrant,
                DueDate = taskData.DueDate,
                IsCompleted = taskData.IsCompleted,
                CreatedAt = taskData.CreatedAt,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Tasks.Add(task);
            tasksImported++;
        }

        await _context.SaveChangesAsync();

        var settingsImported = false;
        if (data.Settings.Count > 0)
        {
            await _settingsService.ImportSettingsAsync(data.Settings);
            settingsImported = true;
        }

        return (tasksImported, settingsImported);
    }
}
