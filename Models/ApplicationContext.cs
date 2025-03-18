using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Feedbacks.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<City> Cities { get; set; } = null!;
        public DbSet<Restaurant> Restaurants { get; set; } = null!;
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
            
            User bob = new User { Id = 1, Email = "bob@gmail.com", Password = "12345", RoleId = adminRole.Id };
            User tom = new User { Id = 2, Email = "tom@gmail.com", Password = "12345", RoleId = adminRole.Id };
            User sam = new User { Id = 3, Email = "sam@gmail.com", Password = "12345", RoleId = userRole.Id };

            City city1 = new City { Id = 1, Name = "Астрахань" };
            City city2 = new City { Id = 2, Name = "Хабаровск" };
            City city3 = new City { Id = 3, Name = "Самара" };

            Restaurant restaurant1 = new Restaurant { Id = 1, Name = "Niki Лавка", CityId = city1.Id };
            Restaurant restaurant2 = new Restaurant { Id = 2, Name = "Сыр. Вино&Мясо", CityId = city1.Id };
            Restaurant restaurant3 = new Restaurant { Id = 3, Name = "Белуга", CityId = city1.Id };

            modelBuilder.Entity<Role>().HasData(adminRole, userRole);
            modelBuilder.Entity<User>().HasData(bob, tom, sam);
            modelBuilder.Entity<City>().HasData(city1, city2, city3);
            modelBuilder.Entity<Restaurant>().HasData(restaurant1, restaurant2, restaurant3);
        }
    }
}
