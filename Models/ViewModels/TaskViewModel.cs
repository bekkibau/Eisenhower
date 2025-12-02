using System.ComponentModel.DataAnnotations;

namespace Eisenhower.Models.ViewModels;

/// <summary>
/// View model for creating/editing tasks
/// </summary>
public class TaskViewModel
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    [Display(Name = "Task Title")]
    public string Title { get; set; } = string.Empty;
    
    [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
    [Display(Name = "Notes")]
    public string? Notes { get; set; }
    
    [Required]
    [Display(Name = "Quadrant")]
    public Quadrant Quadrant { get; set; } = Quadrant.UrgentImportant;
    
    [Display(Name = "Due Date")]
    [DataType(DataType.Date)]
    public DateTime? DueDate { get; set; }
    
    public bool IsCompleted { get; set; }
}

/// <summary>
/// View model for the main matrix view
/// </summary>
public class MatrixViewModel
{
    public List<TaskItem> UrgentImportantTasks { get; set; } = [];
    public List<TaskItem> NotUrgentImportantTasks { get; set; } = [];
    public List<TaskItem> UrgentNotImportantTasks { get; set; } = [];
    public List<TaskItem> NotUrgentNotImportantTasks { get; set; } = [];
    
    public bool ShowCompletedTasks { get; set; } = true;
    public ViewMode ViewMode { get; set; } = ViewMode.Card;
    public ThemeMode Theme { get; set; } = ThemeMode.Light;
    public Quadrant? FilterQuadrant { get; set; }
}

/// <summary>
/// View model for settings page
/// </summary>
public class SettingsViewModel
{
    public ThemeMode Theme { get; set; } = ThemeMode.Light;
    public bool ShowCompletedTasks { get; set; } = true;
    public ViewMode ViewMode { get; set; } = ViewMode.Card;
    
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
}
