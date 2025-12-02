using Eisenhower.Models;
using Eisenhower.Models.ViewModels;

namespace Eisenhower.Services;

/// <summary>
/// Service interface for settings operations
/// </summary>
public interface ISettingsService
{
    Task<ThemeMode> GetThemeAsync();
    Task SetThemeAsync(ThemeMode theme);
    Task<bool> GetShowCompletedTasksAsync();
    Task SetShowCompletedTasksAsync(bool show);
    Task<ViewMode> GetViewModeAsync();
    Task SetViewModeAsync(ViewMode viewMode);
    Task<SettingsViewModel> GetSettingsViewModelAsync();
    Task<Dictionary<string, string>> GetAllSettingsAsDictionaryAsync();
    Task ImportSettingsAsync(Dictionary<string, string> settings);
}
