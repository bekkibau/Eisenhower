using System.ComponentModel.DataAnnotations;

namespace Eisenhower.Models;

/// <summary>
/// Represents a task in the Eisenhower Matrix
/// </summary>
public class TaskItem
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;
    
    [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
    public string? Notes { get; set; }
    
    [Required]
    public Quadrant Quadrant { get; set; } = Quadrant.UrgentImportant;
    
    public DateTime? DueDate { get; set; }
    
    public bool IsCompleted { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
