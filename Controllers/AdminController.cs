using Feedbacks.DTO;
using Feedbacks.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
            ViewBag.Categories = db.RestaurantCategories.ToList();
            return View();
        }

        [HttpPost]
        [Route("AddCity")]
        public IResult AddCity()
        {
            var form = HttpContext.Request.Form;
            if (!form.ContainsKey("Name"))
                return Results.Redirect("/Admin/AdminPanel");
            string name = form["Name"];

            var cities = this.db.Cities.ToList();

            // Проверка на существование города с таким именем
            bool existance_city = cities.Exists(c => c.Name == name);
            if (existance_city || name.Equals(""))
                return Results.Redirect("/Admin/AdminPanel");

            this.db.Cities.Add(new City { Name = name });

            db.SaveChanges();

            return Results.Redirect("/Admin/AdminPanel");
        }

        [HttpPost]
        [Route("AddRestaurant")]
        public IResult AddRestaurant(RestaurantTransferObject rto)
        {
            if (rto.Name.IsNullOrEmpty() || rto.RestaurantImage == null)
                return Results.Redirect("/Admin/AdminPanel");

            string name = rto.Name;
            int cityId = rto.CityId;
            int CategoryId = rto.RestorantCategoryId;
            byte[] imageData;

            using (var binaryReader = new BinaryReader(rto.RestaurantImage.OpenReadStream()))
            {
                imageData = binaryReader.ReadBytes((int)rto.RestaurantImage.Length);
            }

            var restaurants = this.db.Restaurants.ToList();
            var cities = this.db.Cities.ToList();

            // Проверка на существование города
            bool existence_city = cities.Exists(c => c.Id == cityId);
            if (!existence_city)
                return Results.Redirect("/Admin/AdminPanel");

            // Проверка на существование ресторана с таким названием в этом городе
            bool existance_restaurant = restaurants.Exists(r => r.Name == name && r.CityId == cityId);
            if (existance_restaurant)
                return Results.Redirect("/Admin/AdminPanel");
            
            this.db.Restaurants.Add(new Restaurant { Name = name, RestaurantImage = imageData, RestorantCategoryId = CategoryId, Rating = 0, CityId = cityId });

            db.SaveChanges();

            return Results.Redirect("/Admin/AdminPanel");
        }
    }
}
