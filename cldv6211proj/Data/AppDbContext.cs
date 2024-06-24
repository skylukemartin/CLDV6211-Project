using cldv6211proj.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace cldv6211proj.Data
{
    public class AppDbContext : DbContext
    { // ref: https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }
    }
}
