using Feedbacks.Models;
using Feedbacks.Models.enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Feedbacks.Controllers
{
    [Route("Restaurant")]
    public class RestaurantController : Controller
    {
        private ApplicationContext db;
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _appEnvironment;

        public RestaurantController(ILogger<HomeController> logger, ApplicationContext context, IWebHostEnvironment appEnvironment)
        {
            _logger = logger;
            this.db = context;
            _appEnvironment = appEnvironment;
        }

        [Authorize(Roles = "business")]
        [Route("DeletePhotoToRestaurant")]
        public IResult DeletePhotoToRestaurant(int restaurantId, int restaurantImageId)
        {
            Restaurant? restaurant = this.db.Restaurants.ToList().Find(r => r.Id == restaurantId);

            if (restaurant == null || restaurantImageId >= restaurant.RestaurantImage.Count)
                return Results.Redirect("/Restaurant/MyRestaurant");
            
            string fileName = restaurant.RestaurantImage[restaurantImageId];

            System.IO.File.Delete(_appEnvironment.WebRootPath + "/restaurants_photo/" + fileName + ".jpg");

            restaurant.RestaurantImage.RemoveAt(restaurantImageId);
            db.SaveChanges();

            return Results.Redirect("/Restaurant/MyRestaurant");
        }

        [Authorize(Roles = "business")]
        [Route("AddImageToRestaurant")]
        public IResult AddImageToRestaurant(int restaurantId, IFormFile RestaurantImage)
        {
            Restaurant? restaurant = this.db.Restaurants.ToList().Find(r => r.Id == restaurantId);

            if (restaurant == null || RestaurantImage == null)
                return Results.Redirect("/Home/Index");

            string fileName = Guid.NewGuid().ToString();
            
            byte[] buffer = new byte[RestaurantImage.Length];
            RestaurantImage.OpenReadStream().Read(buffer, 0, (int)RestaurantImage.Length);

            System.IO.File.WriteAllBytes(_appEnvironment.WebRootPath + "/restaurants_photo/" + fileName + ".jpg", buffer);

            restaurant.RestaurantImage.Add(fileName);
            db.SaveChanges();
            return Results.Redirect("/Restaurant/MyRestaurant");
        }

        [Route("GetImage")]
        public FileContentResult? GetImage(int restaurantId, int imageId)
        {
            Restaurant? restaurant = db.Restaurants.FirstOrDefault(r => r.Id == restaurantId);

            if (restaurant != null && imageId < restaurant.RestaurantImage.Count)
                return File(System.IO.File.ReadAllBytes(_appEnvironment.WebRootPath + "/restaurants_photo/" + restaurant.RestaurantImage[imageId] + ".jpg"), "jpeg/jpg");

            return null;
        }

        [Route("RestaurantPage")]
        public IActionResult RestaurantPage(int RestaurantId)
        {
            Restaurant? restaurant = this.db.Restaurants.ToList().Find(r => r.Id == RestaurantId);

            if (restaurant == null)
                Results.Redirect("/Home/Index");

            ViewBag.count_photo = restaurant.RestaurantImage.Count;
            ViewBag.reviews = this.db.Reviews.Include(u => u.User).Include(r => r.Reply).ThenInclude(r => r.Author).Where(r => r.Restaurant.Id == restaurant.Id).Where(r => r.Status == Convert.ToInt32(StatusOfReview.Published)).ToList();

            return View(restaurant);
        }

        [Authorize(Roles = "business")]
        [Route("MyRestaurant")]
        public IActionResult MyRestaurant()
        {
            User? user = db.Users.ToList().Find(u => u.Email == HttpContext.User.FindFirst(ClaimTypes.Name)?.Value);
            Restaurant? restaurant = this.db.Restaurants.ToList().Find(r => r.Id == user?.Restaurant?.Id);
            ViewBag.reviews = this.db.Reviews.Include(u => u.User).Include(r => r.Reply).Where(r => r.Restaurant.Id == restaurant.Id).Where(r => r.Status == Convert.ToInt32(StatusOfReview.Published)).ToList();
            ViewBag.count_photo = restaurant.RestaurantImage.Count;
            return View(restaurant);
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
