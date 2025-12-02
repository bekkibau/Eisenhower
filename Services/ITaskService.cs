using Eisenhower.Models;
using Eisenhower.Models.ViewModels;

namespace Eisenhower.Services;

/// <summary>
/// Service interface for task operations
/// </summary>
public interface ITaskService
{
    Task<List<TaskItem>> GetAllTasksAsync();
    Task<List<TaskItem>> GetTasksByQuadrantAsync(Quadrant quadrant);
    Task<TaskItem?> GetTaskByIdAsync(int id);
    Task<TaskItem> CreateTaskAsync(TaskViewModel model);
    Task<TaskItem?> UpdateTaskAsync(int id, TaskViewModel model);
    Task<bool> DeleteTaskAsync(int id);
    Task<bool> ToggleCompletionAsync(int id);
    Task<int> ClearCompletedTasksAsync();
    Task<MatrixViewModel> GetMatrixViewModelAsync(Quadrant? filterQuadrant = null);
}
