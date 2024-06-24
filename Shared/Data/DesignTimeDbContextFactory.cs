using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Shared.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<SharedDbContext>
    {
        public SharedDbContext CreateDbContext(string[] args)
        { // Only used when running: dotnet ef migrations add InitialCreate --output-dir Data/Migrations
            var builder = new DbContextOptionsBuilder<SharedDbContext>();
            var connectionString =
                "Server=localhost,1433;Database=master;User Id=SA;Password=pls?noh4ck54ll0w3d!;Encrypt=false;";
            builder.UseSqlServer(connectionString);

            return new SharedDbContext(builder.Options);
        }
    }
}
