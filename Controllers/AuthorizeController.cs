using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Feedbacks.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

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
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IResult Login(string? returnUrl)
        {
            var people = this.db.Users.ToList();

            var form = HttpContext.Request.Form;

            if (!form.ContainsKey("email") || !form.ContainsKey("password"))
                return Results.BadRequest("Email и/или пароль не установлены");

            string email = form["email"];
            string password = form["password"];

            User? person = people.FirstOrDefault(p => p.Email == email && p.Password == password);

            if (person is null) return Results.Unauthorized(); // 401

            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, person.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, person.RoleId.ToString())
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");

            // установка аутентификационных куки
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
