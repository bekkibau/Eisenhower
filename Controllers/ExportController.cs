using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Eisenhower.Models;
using Eisenhower.Services;

namespace Eisenhower.Controllers;

public class ExportController : Controller
{
    private readonly IExportImportService _exportImportService;
    private readonly ILogger<ExportController> _logger;

    public ExportController(IExportImportService exportImportService, ILogger<ExportController> logger)
    {
        _exportImportService = exportImportService;
        _logger = logger;
    }

    // GET: Export/Download
    public async Task<IActionResult> Download()
    {
        try
        {
            var data = await _exportImportService.ExportDataAsync();
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            
            var fileName = $"eisenhower_backup_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            
            return File(bytes, "application/json", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting data");
            TempData["ErrorMessage"] = "An error occurred while exporting data.";
            return RedirectToAction("Index", "Settings");
        }
    }

    // GET: Export/Import (shows import form)
    public IActionResult Import()
    {
        return View();
    }

    // POST: Export/Import
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Import(IFormFile? file, bool clearExisting = false)
    {
        if (file == null || file.Length == 0)
        {
            TempData["ErrorMessage"] = "Please select a file to import.";
            return View();
        }

        try
        {
            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();
            
            var data = JsonSerializer.Deserialize<ExportData>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data == null)
            {
                TempData["ErrorMessage"] = "Invalid file format.";
                return View();
            }

            var (tasksImported, settingsImported) = await _exportImportService.ImportDataAsync(data, clearExisting);
            
            var message = $"Successfully imported {tasksImported} task(s)";
            if (settingsImported)
            {
                message += " and settings";
            }
            message += ".";
            
            TempData["SuccessMessage"] = message;
            return RedirectToAction("Index", "Home");
        }
        catch (JsonException)
        {
            TempData["ErrorMessage"] = "Invalid JSON file format.";
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing data");
            TempData["ErrorMessage"] = "An error occurred while importing data.";
            return View();
        }
    }
}
