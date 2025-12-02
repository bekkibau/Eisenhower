using Microsoft.AspNetCore.Mvc;
using Eisenhower.Models;
using Eisenhower.Models.ViewModels;
using Eisenhower.Services;

namespace Eisenhower.Controllers;

public class SettingsController : Controller
{
    private readonly ISettingsService _settingsService;
    private readonly ITaskService _taskService;
    private readonly ILogger<SettingsController> _logger;

    public SettingsController(
        ISettingsService settingsService, 
        ITaskService taskService,
        ILogger<SettingsController> logger)
    {
        _settingsService = settingsService;
        _taskService = taskService;
        _logger = logger;
    }

    // GET: Settings
    public async Task<IActionResult> Index()
    {
        var model = await _settingsService.GetSettingsViewModelAsync();
        return View(model);
    }

    // POST: Settings/SetTheme
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetTheme(ThemeMode theme)
    {
        try
        {
            await _settingsService.SetThemeAsync(theme);
            TempData["SuccessMessage"] = "Theme updated successfully!";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting theme");
            TempData["ErrorMessage"] = "An error occurred while updating theme.";
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: Settings/SetShowCompleted
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetShowCompleted(bool show)
    {
        try
        {
            await _settingsService.SetShowCompletedTasksAsync(show);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting show completed");
            TempData["ErrorMessage"] = "An error occurred while updating settings.";
        }

        return RedirectToAction("Index", "Home");
    }

    // POST: Settings/SetViewMode
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetViewMode(ViewMode viewMode)
    {
        try
        {
            await _settingsService.SetViewModeAsync(viewMode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting view mode");
            TempData["ErrorMessage"] = "An error occurred while updating settings.";
        }

        return RedirectToAction("Index", "Home");
    }

    // POST: Settings/ClearCompleted
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ClearCompleted()
    {
        try
        {
            var count = await _taskService.ClearCompletedTasksAsync();
            TempData["SuccessMessage"] = $"Cleared {count} completed task(s).";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing completed tasks");
            TempData["ErrorMessage"] = "An error occurred while clearing tasks.";
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: Settings/SetThemeAjax - For AJAX calls
    [HttpPost]
    public async Task<IActionResult> SetThemeAjax([FromBody] ThemeRequest request)
    {
        try
        {
            await _settingsService.SetThemeAsync(request.Theme);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting theme");
            return Json(new { success = false, message = "An error occurred." });
        }
    }

    // POST: Settings/SetShowCompletedAjax - For AJAX calls
    [HttpPost]
    public async Task<IActionResult> SetShowCompletedAjax([FromBody] ShowCompletedRequest request)
    {
        try
        {
            await _settingsService.SetShowCompletedTasksAsync(request.Show);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting show completed");
            return Json(new { success = false, message = "An error occurred." });
        }
    }

    // POST: Settings/SetViewModeAjax - For AJAX calls
    [HttpPost]
    public async Task<IActionResult> SetViewModeAjax([FromBody] ViewModeRequest request)
    {
        try
        {
            await _settingsService.SetViewModeAsync(request.ViewMode);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting view mode");
            return Json(new { success = false, message = "An error occurred." });
        }
    }
}

public class ThemeRequest
{
    public ThemeMode Theme { get; set; }
}

public class ShowCompletedRequest
{
    public bool Show { get; set; }
}

public class ViewModeRequest
{
    public ViewMode ViewMode { get; set; }
}
