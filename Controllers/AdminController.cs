﻿using Feedbacks.DTO;
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
        public IResult AddRestaurant(RestaurantTransferObject rto)
        {
            if (rto.Name.IsNullOrEmpty() || rto.RestaurantImage == null)
                return Results.Redirect("/Admin/AdminPanel");

            string name = rto.Name;
            int cityId = rto.CityId;
            int CategoryId = rto.RestorantCategoryId;
            byte[] imageData;
            // считываем переданный файл в массив байтов
            using (var binaryReader = new BinaryReader(rto.RestaurantImage.OpenReadStream()))
            {
                imageData = binaryReader.ReadBytes((int)rto.RestaurantImage.Length);
            }

            var restaurants = this.db.Restaurants.ToList();
            var cities = this.db.Cities.ToList();
            // Проверка на существование ресторана
            foreach (var restaurant in restaurants) {
                if (restaurant.Name.Equals(name)) {
                    return Results.Redirect("/Admin/AdminPanel");
                }
            }
            // Проверка на существование города
            bool fl = false;
            foreach (var city in cities) {
                if (city.Id == cityId) {
                    fl = true;
                }
            }
            if (!fl)
                return Results.Redirect("/Admin/AdminPanel");
            
            this.db.Restaurants.Add(new Restaurant { Name = name, RestaurantImage = imageData, RestorantCategoryId = CategoryId, Rating = 0, CityId = cityId });

            db.SaveChanges();

            return Results.Redirect("/Admin/AdminPanel");
        }
    }
}
