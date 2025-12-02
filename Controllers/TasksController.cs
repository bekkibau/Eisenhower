using Microsoft.AspNetCore.Mvc;
using Eisenhower.Models;
using Eisenhower.Models.ViewModels;
using Eisenhower.Services;

namespace Eisenhower.Controllers;

public class TasksController : Controller
{
    private readonly ITaskService _taskService;
    private readonly ILogger<TasksController> _logger;

    public TasksController(ITaskService taskService, ILogger<TasksController> logger)
    {
        _taskService = taskService;
        _logger = logger;
    }

    // GET: Tasks/Create
    public IActionResult Create(Quadrant? quadrant = null)
    {
        var model = new TaskViewModel
        {
            Quadrant = quadrant ?? Quadrant.UrgentImportant
        };
        return View(model);
    }

    // POST: Tasks/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TaskViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _taskService.CreateTaskAsync(model);
            TempData["SuccessMessage"] = "Task created successfully!";
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating task");
            ModelState.AddModelError("", "An error occurred while creating the task.");
            return View(model);
        }
    }

    // GET: Tasks/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var task = await _taskService.GetTaskByIdAsync(id);
        if (task == null)
        {
            return NotFound();
        }

        var model = new TaskViewModel
        {
            Id = task.Id,
            Title = task.Title,
            Notes = task.Notes,
            Quadrant = task.Quadrant,
            DueDate = task.DueDate,
            IsCompleted = task.IsCompleted
        };

        return View(model);
    }

    // POST: Tasks/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TaskViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var result = await _taskService.UpdateTaskAsync(id, model);
            if (result == null)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Task updated successfully!";
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating task {TaskId}", id);
            ModelState.AddModelError("", "An error occurred while updating the task.");
            return View(model);
        }
    }

    // POST: Tasks/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _taskService.DeleteTaskAsync(id);
            if (!result)
            {
                TempData["ErrorMessage"] = "Task not found.";
            }
            else
            {
                TempData["SuccessMessage"] = "Task deleted successfully!";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting task {TaskId}", id);
            TempData["ErrorMessage"] = "An error occurred while deleting the task.";
        }

        return RedirectToAction("Index", "Home");
    }

    // POST: Tasks/Toggle/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int id)
    {
        try
        {
            var result = await _taskService.ToggleCompletionAsync(id);
            if (!result)
            {
                TempData["ErrorMessage"] = "Task not found.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling task {TaskId}", id);
            TempData["ErrorMessage"] = "An error occurred while updating the task.";
        }

        return RedirectToAction("Index", "Home");
    }

    // POST: Tasks/ToggleAjax/5 - For AJAX calls
    [HttpPost]
    public async Task<IActionResult> ToggleAjax(int id)
    {
        try
        {
            var result = await _taskService.ToggleCompletionAsync(id);
            if (!result)
            {
                return Json(new { success = false, message = "Task not found." });
            }
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling task {TaskId}", id);
            return Json(new { success = false, message = "An error occurred." });
        }
    }

    // POST: Tasks/DeleteAjax/5 - For AJAX calls
    [HttpPost]
    public async Task<IActionResult> DeleteAjax(int id)
    {
        try
        {
            var result = await _taskService.DeleteTaskAsync(id);
            if (!result)
            {
                return Json(new { success = false, message = "Task not found." });
            }
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting task {TaskId}", id);
            return Json(new { success = false, message = "An error occurred." });
        }
    }

    // POST: Tasks/MoveToQuadrant - For drag and drop
    [HttpPost]
    public async Task<IActionResult> MoveToQuadrant([FromBody] MoveTaskRequest request)
    {
        try
        {
            var task = await _taskService.GetTaskByIdAsync(request.TaskId);
            if (task == null)
            {
                return Json(new { success = false, message = "Task not found." });
            }

            var model = new TaskViewModel
            {
                Id = task.Id,
                Title = task.Title,
                Notes = task.Notes,
                Quadrant = (Quadrant)request.Quadrant,
                DueDate = task.DueDate,
                IsCompleted = task.IsCompleted
            };

            await _taskService.UpdateTaskAsync(request.TaskId, model);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving task {TaskId} to quadrant {Quadrant}", request.TaskId, request.Quadrant);
            return Json(new { success = false, message = "An error occurred." });
        }
    }
}

public class MoveTaskRequest
{
    public int TaskId { get; set; }
    public int Quadrant { get; set; }
}
