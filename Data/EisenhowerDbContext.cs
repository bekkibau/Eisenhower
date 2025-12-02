using Microsoft.EntityFrameworkCore;
using Eisenhower.Models;

namespace Eisenhower.Data;

/// <summary>
/// Entity Framework DbContext for the Eisenhower Matrix application
/// </summary>
public class EisenhowerDbContext : DbContext
{
    public EisenhowerDbContext(DbContextOptions<EisenhowerDbContext> options) 
        : base(options)
    {
    }
    
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<Settings> Settings => Set<Settings>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure TaskItem
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.Quadrant).IsRequired();
            entity.HasIndex(e => e.Quadrant);
            entity.HasIndex(e => e.IsCompleted);
        });
        
        // Configure Settings
        modelBuilder.Entity<Settings>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Key).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Value).IsRequired().HasMaxLength(500);
            entity.HasIndex(e => e.Key).IsUnique();
        });
        
        // Seed default settings
        modelBuilder.Entity<Settings>().HasData(
            new Settings { Id = 1, Key = SettingsKeys.Theme, Value = "Light" },
            new Settings { Id = 2, Key = SettingsKeys.ShowCompletedTasks, Value = "true" },
            new Settings { Id = 3, Key = SettingsKeys.ViewMode, Value = "Card" }
        );
    }
}
