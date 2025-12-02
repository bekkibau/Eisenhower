using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Eisenhower.Models;
using Eisenhower.Services;

namespace Eisenhower.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ITaskService _taskService;

    public HomeController(ILogger<HomeController> logger, ITaskService taskService)
    {
        _logger = logger;
        _taskService = taskService;
    }

    public async Task<IActionResult> Index(Quadrant? quadrant = null)
    {
        var model = await _taskService.GetMatrixViewModelAsync(quadrant);
        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
