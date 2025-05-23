﻿using System.Diagnostics;
using System.Security.Claims;
using Feedbacks.Models;
using Feedbacks.DTO;
using Feedbacks.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;


namespace Feedbacks.Controllers
{
    public class AccountController : Controller
    {
        ApplicationContext db;
        private readonly IWebHostEnvironment _appEnvironment;
        
        public AccountController(ApplicationContext context, IWebHostEnvironment appEnvironment)
        {
            this.db = context;
            _appEnvironment = appEnvironment;
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.Cities = db.Cities.ToList();
            ViewBag.RestaurantCategories = db.RestaurantCategories.ToList();

            return View();
        }

        public IResult RegisterBusinessUserAndRestaurant(AccountDTO account, RestaurantDTO restaurant)
        {
            User? user = GetUser(account);

            string name = restaurant.Name;
            int cityId = restaurant.CityId;
            int CategoryId = restaurant.RestorantCategoryId;

            Restaurant new_restaurant = new Restaurant { Name = name, RestorantCategoryId = CategoryId, Rating = 0, CityId = cityId, Activated = false, Address = "", Description = "" };
            
            if (user == null)
            {
                return Results.BadRequest(new { message = "User is already registered" });
            }
            user.RoleId = db.Roles.ToList().Find(r => r.Name == "business").Id;
            user.Restaurant = new_restaurant;
            // User по умолчанию не активирован
            this.db.Users.Add(user);

            if (String.IsNullOrEmpty(restaurant.Name) || restaurant.RestaurantImage == null)
                return Results.BadRequest(new { message = "Invalid data" });

            string fileName = Guid.NewGuid().ToString();

            byte[] buffer = new byte[restaurant.RestaurantImage.Length];
            restaurant.RestaurantImage.OpenReadStream().Read(buffer, 0, (int)restaurant.RestaurantImage.Length);

            System.IO.File.WriteAllBytes(_appEnvironment.WebRootPath + "/restaurants_photo/" + fileName + ".jpg", buffer);

            var restaurants = db.Restaurants.ToList();
            var cities = db.Cities.ToList();

            // Проверка на существование города
            bool existence_city = cities.Exists(c => c.Id == cityId);
            if (!existence_city)
                return Results.BadRequest(new { message = "Invalid city" });

            // Проверка на существование ресторана с таким названием в этом городе
            bool existance_restaurant = restaurants.Exists(r => r.Name == name && r.CityId == cityId);
            if (existance_restaurant)
                return Results.BadRequest(new { message = "Restaurant with this name already exists in this city" });

            new_restaurant.RestaurantImage.Add(fileName);
            this.db.Restaurants.Add(new_restaurant);

            db.SaveChanges();
                      
            return Results.Redirect("/");
        }

        private User? GetUser(AccountDTO ato)
        {
            var users = this.db.Users.ToList();
            if (ato.Email == null || users.Find(u => u.Email == ato.Email) != null)
                return null;

            if (ato?.CityId == null || ato?.Password == null)
                return null;

            City? city = this.db.Cities.ToList().Find(c => c.Id == ato.CityId);
            if (city != null)
            {
                User user = new User { Email = ato.Email, City = city, Password = PasswordHasherHelper.HashString(ato.Password), RoleId = db.Roles.ToList().Find(r => r.Name == "user").Id, Activated = false };
                return user;
            }
            return null;
        }

        [HttpPost]
        public IResult Register(AccountDTO ato)
        {
            User? user = GetUser(ato);
            if (user != null)
            {
                user.Activated = true;
                this.db.Users.Add(user);
                this.db.SaveChanges();
            }
            return Results.Redirect("/");
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
            if (password != null)
                password = PasswordHasherHelper.HashString(password);

            User? person = people.FirstOrDefault(p => p.Email == email && p.Password == password);
            var claims = new List<Claim> { };
            if (person != null)
            {
                if (!person.Activated)
                {
                    return Results.BadRequest("Аккаунт не активирован");
                }
                claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, person.Email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, roles.Find(c => c.Id == person.RoleId).Name),
                    new Claim("city", person.City.Name)
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
