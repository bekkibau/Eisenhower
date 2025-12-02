namespace Eisenhower.Models;

/// <summary>
/// Represents the four quadrants of the Eisenhower Matrix
/// </summary>
public enum Quadrant
{
    /// <summary>
    /// Quadrant 1: Urgent and Important - Do First
    /// </summary>
    UrgentImportant = 0,
    
    /// <summary>
    /// Quadrant 2: Not Urgent but Important - Schedule
    /// </summary>
    NotUrgentImportant = 1,
    
    /// <summary>
    /// Quadrant 3: Urgent but Not Important - Delegate
    /// </summary>
    UrgentNotImportant = 2,
    
    /// <summary>
    /// Quadrant 4: Not Urgent and Not Important - Eliminate
    /// </summary>
    NotUrgentNotImportant = 3
}
