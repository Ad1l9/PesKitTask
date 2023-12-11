using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PesKitTask.Models;
using PesKitTask.ViewModel;

namespace PesKitTask.DAL
{
    public class AppDbContext:IdentityDbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Tag> Tags { get; set; }
		public DbSet<Author> Authors { get; set; }
		public DbSet<ProjectImage> ProjectImages { get; set; }
		public DbSet<Project> Projects { get; set; }
		public DbSet<Department> Departments { get; set; }
		public DbSet<Position> Positions { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
    }
}
