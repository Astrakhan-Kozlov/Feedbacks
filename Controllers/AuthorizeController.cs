using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Feedbacks.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Feedbacks.DTO;

namespace Feedbacks.Controllers
{
    public class AuthorizeController : Controller
    {
        ApplicationContext db;

        public AuthorizeController(ApplicationContext context)
        {
            this.db = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.Cities = db.Cities.ToList();

            return View();
        }

        [HttpPost]
        public IResult Register(AccountTransferObject ato)
        {
            var users = this.db.Users.ToList();
            if (ato.Email == null || users.Find(u => u.Email == ato.Email) != null)
                return Results.Redirect("Home/Index");

            if (ato?.CityId == null || ato?.Password == null)
                return Results.Redirect("Home/Index");
            
            City? city = this.db.Cities.ToList().Find(c => c.Id == ato.CityId);
            if (city != null)
            {
                this.db.Users.Add(new User { Email = ato.Email, City = city, Password = ato.Password, RoleId = db.Roles.ToList().Find(r => r.Name == "user").Id });
                this.db.SaveChanges();
            }

            return Results.Redirect("Home/Index");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IResult Login(string? returnUrl)
        {
            var people = this.db.Users.ToList();
            var roles = this.db.Roles.ToList();
            var cities = this.db.Cities.ToList();

            var form = HttpContext.Request.Form;

            if (!form.ContainsKey("email") || !form.ContainsKey("password"))
                return Results.BadRequest("Email и/или пароль не установлены");

            string? email = form["email"];
            string? password = form["password"];

            User? person = people.FirstOrDefault(p => p.Email == email && p.Password == password);
            User? businessPerson = db.Users.ToList().FirstOrDefault(p => p.Email == email && p.Password == password);
            var claims = new List<Claim> { };
            if (person != null)
            {
                claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, person.Email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, roles.Find(c => c.Id == person.RoleId).Name),
                    new Claim("city", person.City.Name)
                };
            }
            else if (businessPerson != null)
            {
                claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, businessPerson.Email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, roles.Find(c => c.Id == businessPerson.RoleId).Name),
                    new Claim("city", businessPerson.City.Name),
                    new Claim("restaurantId", businessPerson.Restaurant.Id.ToString())
                };
            }
            else
                return Results.Unauthorized(); // 401

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            // установка аутентификационных кук
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            return Results.Redirect(returnUrl ?? "/");
        }

        public IResult logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.Redirect("/");
        }
    }
}
