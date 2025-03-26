using Microsoft.EntityFrameworkCore;
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

            City astrakhan = new City { Id = 1, Name = "Астрахань" };
            City khabarovsk = new City { Id = 2, Name = "Хабаровск" };
            City samara = new City { Id = 3, Name = "Самара" };

            User bob = new User { Id = 1, CityId = astrakhan.Id, Email = "bob@gmail.com", Password = "12345", RoleId = userRole.Id };
            User tom = new User { Id = 2, CityId = astrakhan.Id, Email = "tom@gmail.com", Password = "12345", RoleId = adminRole.Id };
            User sam = new User { Id = 3, CityId = khabarovsk.Id, Email = "sam@gmail.com", Password = "12345", RoleId = userRole.Id };

            RestaurantCategory confectionery = new RestaurantCategory { Id = 1, Name = "Кондитерская" };
            RestaurantCategory kebab_house = new RestaurantCategory { Id = 2, Name = "Шашлычная" };
            RestaurantCategory bakery = new RestaurantCategory { Id = 3, Name = "Пекарня" };
            RestaurantCategory classic_category = new RestaurantCategory { Id = 4, Name = "Классический" };

            Restaurant restaurant1 = new Restaurant { Id = 1, Name = "Niki", RestorantCategoryId = confectionery.Id, Rating =.0, RestaurantImage = File.ReadAllBytes("wwwroot\\restaurants_photo\\Niki.jpg"), CityId = astrakhan.Id };
            Restaurant restaurant2 = new Restaurant { Id = 2, Name = "Сыр. Вино&Мясо", RestorantCategoryId = classic_category.Id, Rating =.0, RestaurantImage = File.ReadAllBytes("wwwroot\\restaurants_photo\\сыр_вино_и_мясо.jpg"), CityId = astrakhan.Id };
            Restaurant restaurant3 = new Restaurant { Id = 3, Name = "Белуга", RestorantCategoryId = classic_category.Id, Rating =.0, RestaurantImage = File.ReadAllBytes("wwwroot\\restaurants_photo\\Белуга.jpg"), CityId = khabarovsk.Id };

            modelBuilder.Entity<Role>().HasData(adminRole, userRole);
            modelBuilder.Entity<User>().HasData(bob, tom, sam);
            modelBuilder.Entity<City>().HasData(astrakhan, khabarovsk, samara);
            modelBuilder.Entity<RestaurantCategory>().HasData(confectionery, kebab_house, bakery, classic_category);
            modelBuilder.Entity<Restaurant>().HasData(restaurant1, restaurant2, restaurant3);
        }
    }
}
