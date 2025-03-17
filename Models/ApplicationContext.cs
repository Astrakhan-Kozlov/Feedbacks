using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Feedbacks.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Role adminRole = new Role { Id=1, Name = "admin" };
            Role userRole = new Role { Id=2, Name = "user" };
            
            User bob = new User { Id=1, Email = "bob@gmail.com", Password="12345", RoleId = adminRole.Id };
            User tom = new User { Id=2, Email = "tom@gmail.com", Password="12345", RoleId = adminRole.Id };
            User sam = new User { Id=3, Email = "sam@gmail.com", Password="12345", RoleId = userRole.Id };

            
            modelBuilder.Entity<Role>().HasData(adminRole, userRole);
            modelBuilder.Entity<User>().HasData(bob, tom, sam);
        }
    }
}
