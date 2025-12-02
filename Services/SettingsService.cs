using Microsoft.EntityFrameworkCore;
using Eisenhower.Data;
using Eisenhower.Models;
using Eisenhower.Models.ViewModels;

namespace Eisenhower.Services;

/// <summary>
/// Service implementation for settings operations
/// </summary>
public class SettingsService : ISettingsService
{
    private readonly EisenhowerDbContext _context;

    public SettingsService(EisenhowerDbContext context)
    {
        _context = context;
    }

    public async Task<ThemeMode> GetThemeAsync()
    {
        var setting = await _context.Settings
            .FirstOrDefaultAsync(s => s.Key == SettingsKeys.Theme);
        
        if (setting != null && Enum.TryParse<ThemeMode>(setting.Value, out var theme))
        {
            return theme;
        }
        return ThemeMode.Light;
    }

    public async Task SetThemeAsync(ThemeMode theme)
    {
        await SetSettingAsync(SettingsKeys.Theme, theme.ToString());
    }

    public async Task<bool> GetShowCompletedTasksAsync()
    {
        var setting = await _context.Settings
            .FirstOrDefaultAsync(s => s.Key == SettingsKeys.ShowCompletedTasks);
        
        if (setting != null && bool.TryParse(setting.Value, out var show))
        {
            return show;
        }
        return true;
    }

    public async Task SetShowCompletedTasksAsync(bool show)
    {
        await SetSettingAsync(SettingsKeys.ShowCompletedTasks, show.ToString().ToLower());
    }

    public async Task<ViewMode> GetViewModeAsync()
    {
        var setting = await _context.Settings
            .FirstOrDefaultAsync(s => s.Key == SettingsKeys.ViewMode);
        
        if (setting != null && Enum.TryParse<ViewMode>(setting.Value, out var viewMode))
        {
            return viewMode;
        }
        return ViewMode.Card;
    }

    public async Task SetViewModeAsync(ViewMode viewMode)
    {
        await SetSettingAsync(SettingsKeys.ViewMode, viewMode.ToString());
    }

    public async Task<SettingsViewModel> GetSettingsViewModelAsync()
    {
        var totalTasks = await _context.Tasks.CountAsync();
        var completedTasks = await _context.Tasks.CountAsync(t => t.IsCompleted);

        return new SettingsViewModel
        {
            Theme = await GetThemeAsync(),
            ShowCompletedTasks = await GetShowCompletedTasksAsync(),
            ViewMode = await GetViewModeAsync(),
            TotalTasks = totalTasks,
            CompletedTasks = completedTasks
        };
    }

    public async Task<Dictionary<string, string>> GetAllSettingsAsDictionaryAsync()
    {
        var settings = await _context.Settings.ToListAsync();
        return settings.ToDictionary(s => s.Key, s => s.Value);
    }

    public async Task ImportSettingsAsync(Dictionary<string, string> settings)
    {
        foreach (var kvp in settings)
        {
            await SetSettingAsync(kvp.Key, kvp.Value);
        }
    }

    private async Task SetSettingAsync(string key, string value)
    {
        var setting = await _context.Settings
            .FirstOrDefaultAsync(s => s.Key == key);

        if (setting == null)
        {
            setting = new Settings { Key = key, Value = value };
            _context.Settings.Add(setting);
        }
        else
        {
            setting.Value = value;
        }

        await _context.SaveChangesAsync();
    }
}
