using Eisenhower.Models;

namespace Eisenhower.Services;

/// <summary>
/// Service interface for export/import operations
/// </summary>
public interface IExportImportService
{
    Task<ExportData> ExportDataAsync();
    Task<(int tasksImported, bool settingsImported)> ImportDataAsync(ExportData data, bool clearExisting = false);
}
