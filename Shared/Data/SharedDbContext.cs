using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Shared.Data
{
    public class SharedDbContext : DbContext
    { // ref: https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }

        public SharedDbContext(DbContextOptions<SharedDbContext> options)
            : base(options) { }
    }
}
