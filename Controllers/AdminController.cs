using Feedbacks.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Feedbacks.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("Admin")]
    public class AdminController : Controller
    {
        ApplicationContext db;

        public AdminController(ApplicationContext context)
        {
            this.db = context;
        }

        [HttpGet]
        [Route("AdminPanel")]
        public IActionResult AdminPanel()
        {
            ViewData["Username"] = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            ViewData["Role"] = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            ViewBag.cities = db.Cities.ToList();
            ViewBag.restaurants = db.Restaurants.ToList();
            return View();
        }

        [HttpPost]
        [Route("AddCity")]
        public IResult AddCity()
        {
            var form = HttpContext.Request.Form;
            if (!form.ContainsKey("Name"))
                return Results.Redirect("/Admin/AdminPanel");
            string? name = form["Name"];

            var cities = this.db.Cities.ToList();

            foreach (var city in cities) {
                if (city.Name.Equals(name)) {
                    return Results.Redirect("/Admin/AdminPanel");
                }
            }
            this.db.Cities.Add(new City { Name = name });

            db.SaveChanges();

            return Results.Redirect("/Admin/AdminPanel");
        }

        [HttpPost]
        [Route("AddRestaurant")]
        public IResult AddRestaurant()
        {
            var form = HttpContext.Request.Form;

            if (!form.ContainsKey("Name"))
                return Results.Redirect("/Admin/AdminPanel");

            string? name = form["Name"];
            int cityId = Int32.Parse(form["CityId"]);

            var restaurants = this.db.Restaurants.ToList();
            var cities = this.db.Cities.ToList();
            // Проверка на существование ресторана
            foreach (var restaurant in restaurants)
            {
                if (restaurant.Name.Equals(name))
                {
                    return Results.Redirect("/Admin/AdminPanel");
                }
            }
            // Проверка на существование города
            bool fl = false;
            foreach (var city in cities)
            {
                if (city.Id == cityId)
                {
                    fl = true;
                }
            }
            if (!fl)
            {
                return Results.Redirect("/Admin/AdminPanel");
            }

            this.db.Restaurants.Add(new Restaurant { Name = name, CityId = cityId });

            db.SaveChanges();

            return Results.Redirect("/Admin/AdminPanel");
        }
    }
}
