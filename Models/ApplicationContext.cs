﻿using Microsoft.EntityFrameworkCore;
using Feedbacks.Helpers;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Feedbacks.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<RestaurantCategory> RestaurantCategories { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Reply> Replies { get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Role adminRole = new Role { Id = 1, Name = "admin" };
            Role userRole = new Role { Id = 2, Name = "user" };
            Role businessRole = new Role { Id = 3, Name = "business" };

            City astrakhan = new City { Id = 1, Name = "Астрахань" };
            City khabarovsk = new City { Id = 2, Name = "Хабаровск" };
            City samara = new City { Id = 3, Name = "Самара" };

            string pass = PasswordHasherHelper.HashString("12345");

            User bob = new User { Id = 1, CityId = astrakhan.Id, Email = "bob@gmail.com", Password = pass, RoleId = userRole.Id };
            User tom = new User { Id = 2, CityId = astrakhan.Id, Email = "tom@gmail.com", Password = pass, RoleId = adminRole.Id };
            User sam = new User { Id = 3, CityId = khabarovsk.Id, Email = "sam@gmail.com", Password = pass, RoleId = userRole.Id };
            User rob = new User { Id = 4, CityId = astrakhan.Id, Email = "rob@gmail.com", Password = pass, RoleId = businessRole.Id };
            User rob2 = new User { Id = 5, CityId = astrakhan.Id, Email = "rob2@gmail.com", Password = pass, RoleId = businessRole.Id };
            User rob3 = new User { Id = 6, CityId = khabarovsk.Id, Email = "rob3@gmail.com", Password = pass, RoleId = businessRole.Id };

            RestaurantCategory confectionery = new RestaurantCategory { Id = 1, Name = "Кондитерская" };
            RestaurantCategory kebab_house = new RestaurantCategory { Id = 2, Name = "Шашлычная" };
            RestaurantCategory bakery = new RestaurantCategory { Id = 3, Name = "Пекарня" };
            RestaurantCategory classic_category = new RestaurantCategory { Id = 4, Name = "Классический" };

            Restaurant restaurant1 = new Restaurant { Id = 1, Activated = true, Name = "Niki", RestorantCategoryId = confectionery.Id, Rating =.0, CityId = astrakhan.Id, Description="", Address="" };
            Restaurant restaurant2 = new Restaurant { Id = 2, Activated = true, Name = "Сыр. Вино&Мясо", RestorantCategoryId = classic_category.Id, Rating =.0, CityId = astrakhan.Id, Description="", Address="" };
            Restaurant restaurant3 = new Restaurant { Id = 3, Activated = true, Name = "Белуга", RestorantCategoryId = classic_category.Id, Rating =.0, CityId = khabarovsk.Id, Description="", Address="" };
            restaurant1.RestaurantImage.Add("8dad5855-9251-4e48-af6c-eee068ba103c");
            restaurant2.RestaurantImage.Add("b08d7289-700f-41df-b2d0-463cadcd7fd8");
            restaurant3.RestaurantImage.Add("8721db32-5b48-40f0-9955-874b4d711ca2");
            
            rob.RestaurantId = restaurant1.Id;
            rob2.RestaurantId = restaurant2.Id;
            rob3.RestaurantId = restaurant3.Id;

            modelBuilder.Entity<Role>().HasData(adminRole, userRole, businessRole);
            modelBuilder.Entity<User>().HasData(bob, tom, sam, rob, rob2, rob3);
            modelBuilder.Entity<City>().HasData(astrakhan, khabarovsk, samara);
            modelBuilder.Entity<RestaurantCategory>().HasData(confectionery, kebab_house, bakery, classic_category);
            //modelBuilder.Entity<Restaurant>().HasOne(u => u.User).WithOne().OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Restaurant>().HasData(restaurant1, restaurant2, restaurant3);
            modelBuilder.Entity<Review>().HasOne(r => r.Restaurant).WithMany(r => r.Reviwes).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Reply>().HasOne(r => r.Review).WithOne(r => r.Reply).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Reply>().HasOne(a => a.Author).WithMany().OnDelete(DeleteBehavior.NoAction);
        }
    }
}
