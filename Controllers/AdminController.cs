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
            return View();
        }

        public IResult AddCity()
        {
            var form = HttpContext.Request.Form;

            if (!form.ContainsKey("Name"))
                return Results.BadRequest();

            string? name = form["Name"];

            var cities = this.db.Cities.ToList();
            // Добавить проверку на уже имеющиеся города

            this.db.Cities.Add(new City { Name = name });

            db.SaveChanges();

            return Results.Redirect("/Admin/AdminPanel");
        }
    }
}
