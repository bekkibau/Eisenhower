namespace Eisenhower.Models;

/// <summary>
/// Data structure for export/import functionality
/// </summary>
public class ExportData
{
    public string Version { get; set; } = "1.0";
    public DateTime ExportedAt { get; set; } = DateTime.UtcNow;
    public List<TaskExportItem> Tasks { get; set; } = [];
    public Dictionary<string, string> Settings { get; set; } = [];
}

/// <summary>
/// Task data for export (without internal IDs)
/// </summary>
public class TaskExportItem
{
    public string Title { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public int Quadrant { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
}
