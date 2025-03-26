using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Feedbacks.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Feedbacks.DTO;
using Microsoft.EntityFrameworkCore;
using Feedbacks.Models.enums;

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
        if (User.Identity.IsAuthenticated)
        {
            ViewData["Username"] = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            ViewData["Role"] = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
                        
            int id_users_city = 0;
            Int32.TryParse(HttpContext.User.Claims.Where(c => c.Type == "city").Select(c => c.Value).SingleOrDefault(), out id_users_city);
            restaurants = db.Restaurants.ToList().Where(r => r.CityId == id_users_city);
        }
        else // Если пользователь не авторизован, то отображаем все рестораны
        {
            restaurants = db.Restaurants.ToList();
        }

        return View(restaurants);
    }

    public FileContentResult? GetImage(int restaurantId)
    {
        Restaurant? restaurant = db.Restaurants.FirstOrDefault(r => r.Id == restaurantId);

        if (restaurant != null)
            return File(restaurant.RestaurantImage, "jpeg/jpg");

        return null;
    }

    public IActionResult RestaurantPage(int RestaurantId)
    {
        ViewData["Username"] = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
        ViewData["Role"] = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

        Restaurant? restaurant = this.db.Restaurants.ToList().Find(r => r.Id == RestaurantId);
        
        if (restaurant == null)
            Results.Redirect("/Home/Index");

        ViewBag.reviews = this.db.Reviews.Include(u => u.User).Where(r => r.Restaurant.Id == restaurant.Id).Where(r => r.Status == Convert.ToInt32(StatusOfReview.Published)).ToList();

        return View(restaurant);
    }

    [Authorize]
    [HttpGet]
    public IActionResult Reviews()
    {
        string? emailUser = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
        ViewData["Username"] = emailUser;
        ViewData["Role"] = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

        var user = db.Users.ToList().Find(u => u.Email == emailUser);
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
