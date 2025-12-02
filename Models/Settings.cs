using System.ComponentModel.DataAnnotations;

namespace Eisenhower.Models;

/// <summary>
/// Represents application settings stored in the database
/// </summary>
public class Settings
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Key { get; set; } = string.Empty;
    
    [Required]
    [StringLength(500)]
    public string Value { get; set; } = string.Empty;
}

/// <summary>
/// Strongly-typed settings keys
/// </summary>
public static class SettingsKeys
{
    public const string Theme = "Theme";
    public const string ShowCompletedTasks = "ShowCompletedTasks";
    public const string ViewMode = "ViewMode";
}

/// <summary>
/// Available themes
/// </summary>
public enum ThemeMode
{
    Light,
    Dark,
    System
}

/// <summary>
/// Available view modes
/// </summary>
public enum ViewMode
{
    Card,
    List
}
