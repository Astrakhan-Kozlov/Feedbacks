using Feedbacks.DTO;
using Feedbacks.Models;
using Feedbacks.Models.enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
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
            ViewBag.cities = db.Cities.ToList();
            ViewBag.restaurants = db.Restaurants.ToList();
            ViewBag.RestaurantCategories = db.RestaurantCategories.ToList();
            var role = db.Roles.ToList().Find(r => r.Name == "business");
            ViewBag.businessUsers = db.Users.ToList().Where(u => u.RoleId == role.Id);

            return View();
        }

        [HttpGet]
        [Route("ModerateReviews")]
        public IActionResult ModerateReviews()
        {
            string? userEmail = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            User? user = this.db.Users.ToList().Find(u => u.Email == userEmail);

            List<Review> reviews = db.Reviews.Include(u => u.User).Include(r => r.Restaurant).Where(r => r.Status == Convert.ToInt32(StatusOfReview.NotModerated))
                .Where(r => r.User.CityId == user.CityId).ToList(); // Выборка отзывов только с того же города, с которого администратор + неотмодерированные
            return View(reviews);
        }

        [HttpGet]
        [Route("CommitReview")]
        public IResult CommitReview(string type, int ReviewId)
        {
            Review? review = this.db.Reviews.ToList().Find(r => r.Id == ReviewId);
            if (review != null)
            {
                if (type.Equals("publish"))
                {
                    review.Status = Convert.ToInt32(StatusOfReview.Published);
                    Restaurant? restaurant = db.Restaurants.ToList().Find(r => r.Id == review.Restaurant.Id);
                    if (restaurant == null)
                        return Results.Redirect("/Admin/ModerateReviews");
                    double sum = 0;
                    List<Review> list = restaurant.Reviwes.ToList();
                    foreach (var elem in list)
                        sum += elem.Rating;
                    restaurant.Rating = sum / restaurant.Reviwes.Count;
                    db.SaveChanges();
                }                   
                else if (type.Equals("reject"))
                    review.Status = Convert.ToInt32(StatusOfReview.Rejected);
                db.SaveChanges();
            }

            return Results.Redirect("/Admin/ModerateReviews");
        }

        [HttpGet]
        [Route("ChangeStatusRestaurant")]
        public IResult ChangeStatusRestaurant(int restaurant_id)
        {
            Restaurant? restaurant = db.Restaurants.ToList().Find(r => r.Id == restaurant_id);
            if (restaurant == null)
            {
                return Results.BadRequest();
            }
            restaurant.Activated = !restaurant.Activated;
            db.SaveChanges();
            return Results.Redirect("/Admin/AdminPanel");
        }

        [HttpGet]
        [Route("ChangeStatusUser")]
        public IResult ChangeStatusUser(int user_id)
        {
            User? user = db.Users.ToList().Find(u => u.Id == user_id);
            if (user == null)
            {
                return Results.BadRequest();
            }
            user.Activated = !user.Activated;
            db.SaveChanges();
            return Results.Redirect("/Admin/AdminPanel");
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
        [Route("AddCategory")]
        public IResult AddCategory(string name)
        {
            if (name.IsNullOrEmpty())
                return Results.Redirect("/Admin/AdminPanel");

            this.db.RestaurantCategories.Add(new RestaurantCategory { Name = name});
            db.SaveChanges();

            return Results.Redirect("/Admin/AdminPanel");
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
    }
}
