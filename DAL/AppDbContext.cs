using Microsoft.EntityFrameworkCore;
using PesKitTask.Models;

namespace PesKitTask.DAL
{
    public class AppDbContext:DbContext
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
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
    }
}
