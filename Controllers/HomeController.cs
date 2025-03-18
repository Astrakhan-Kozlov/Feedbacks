using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Feedbacks.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Feedbacks.Controllers;

public class HomeController : Controller
{
    private ApplicationContext db;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, ApplicationContext context)
    {
        _logger = logger;
        this.db = context;
    }

    public IActionResult Index()
    {
        bool is_authorized = false;
        if (HttpContext.User.FindFirst(ClaimTypes.Name) != null)
            is_authorized = true;

        if (is_authorized)
        {
            ViewData["Username"] = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            ViewData["Role"] = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            int id_users_city = 0;
            Int32.TryParse(HttpContext.User.Claims.Where(c => c.Type == "city").Select(c => c.Value).SingleOrDefault(), out id_users_city);
            ViewBag.Restaurants = db.Restaurants.ToList().Where(r => r.CityId == id_users_city);
        }
        else // Если пользователь не авторизован, то отображаем все рестораны
        {
            ViewBag.Restaurants = db.Restaurants.ToList();
        }

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
