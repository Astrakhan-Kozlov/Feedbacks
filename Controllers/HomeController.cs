using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Feedbacks.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Feedbacks.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        ViewData["Title"] = "Главная";
        ViewData["Username"] = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
