using Feedbacks.Models;
using Feedbacks.Models.enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Feedbacks.Controllers
{
    [Authorize(Roles = "business")]
    [Route("Business")]
    public class BusinessController : Controller
    {
        private ApplicationContext db;
        private readonly ILogger<BusinessController> _logger;

        public BusinessController(ILogger<BusinessController> logger, ApplicationContext context)
        {
            _logger = logger;
            this.db = context;
        }

        [Route("MyRestaurant")]
        public IActionResult MyRestaurant()
        {
            User? user = db.Users.ToList().Find(u => u.Email == HttpContext.User.FindFirst(ClaimTypes.Name)?.Value);
            Restaurant? restaurant = this.db.Restaurants.ToList().Find(r => r.Id == user?.Restaurant?.Id);
            ViewBag.reviews = this.db.Reviews.Include(u => u.User).Include(r => r.Reply).Where(r => r.Restaurant.Id == restaurant.Id).Where(r => r.Status == Convert.ToInt32(StatusOfReview.Published)).ToList();
            //if (restaurant != null)
            return View(restaurant);
            //return Results.Redirect("/");
        }

        [Route("Reply")]
        [HttpGet]
        public IActionResult Reply(int ReviewId)
        {
            ViewBag.reviewId = ReviewId;
            // тут найти отзыв
            Review? review = this.db.Reviews.ToList().Find(r => r.Id == ReviewId);

            return View(review);
        }

        [HttpPost]
        [Route("ReplyReview")]
        public IResult ReplyReview(int reviewId, string text)
        {
            string? userEmail = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            User? businessUser = this.db.Users.ToList().Find(u => u.Email == userEmail);

            if (businessUser == null)
                return Results.Redirect("/");

            this.db.Replies.Add(new Reply { ReviewId = reviewId, Author = businessUser, Text = text });
            db.SaveChanges();

            return Results.Redirect("/");
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
