using Microsoft.EntityFrameworkCore;
using PesKitTask.Models;

namespace PesKitTask.DAL
{
    public class AppDbContext:DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Author> Authors { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
    }
}
