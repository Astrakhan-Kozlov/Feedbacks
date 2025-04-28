using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Feedbacks.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Feedbacks.DTO;
using Microsoft.EntityFrameworkCore;
using Feedbacks.Models.enums;
using Microsoft.AspNetCore.Mvc.Filters;

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
        IEnumerable<Restaurant> restaurants;
        ViewBag.Categories = db.RestaurantCategories.ToList();
        ViewBag.Cities = db.Cities.ToList();
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            restaurants = db.Restaurants.ToList().Where(r => r.City.Name == HttpContext.User.FindFirst("city")?.Value);
        }
        else
        {
            restaurants = db.Restaurants.ToList();
        }

        return View(restaurants);
    }

    [Authorize]
    [HttpGet]
    public IActionResult Reviews()
    {
        string? emailUser = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
        var user = db.Users.ToList().Find(u => u.Email == emailUser);
        ViewBag.dct = new Dictionary<string, string> { { "0", "Не проверен" }, { "1", "Опубликован" }, { "2", "Отклонен" } };
        List<Review> reviews = db.Reviews.Include(u => u.User).Where(r => r.UserId == user.Id).ToList(); // Только свои отзывы
        ViewBag.restaurants = this.db.Restaurants.ToList().Where(r => r.CityId == user.CityId); // Отбор ресторанов с этого же города

        return View(reviews);
    }

    [Authorize]
    [HttpPost]
    public IResult AddReview(ReviewTransferObject rto)
    {
        var userEmail = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
        User? user = db.Users.ToList().Find(u => u.Email == userEmail);
        Restaurant? restaurant = db.Restaurants.ToList().Find(r => r.Id == rto.RestaurantId);

        if (user != null && restaurant != null && Enumerable.Range(0, 10).Contains(rto.Rating))
        {
            Review review = new Review { User = user, Text = rto.Text, Title = rto.Title, 
                Restaurant = restaurant, Status = Convert.ToInt32(StatusOfReview.NotModerated), Rating = rto.Rating };

            this.db.Reviews.Add(review);

            this.db.SaveChanges();
        }

        return Results.Redirect("/Home/Reviews");
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        base.OnActionExecuted(context);
        if (context.Result is ViewResult)
        {
            ViewData["Username"] = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            ViewData["Role"] = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            ViewData["City"] = HttpContext.User.FindFirst("city")?.Value;
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
