using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace RentifyApi.Data
{
    // Provides a design-time factory so `dotnet ef` can create the DbContext.
    // By default this factory will use a local SQLite file for design-time operations.
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // Look for an environment variable to force using MySQL at design time
            var useMySql = Environment.GetEnvironmentVariable("EF_DESIGN_USE_MYSQL");

            if (!string.IsNullOrEmpty(useMySql) && useMySql.ToLower() == "true")
            {
                // read connection string from appsettings.json
                var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                var conn = config.GetConnectionString("DefaultConnection");
                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                optionsBuilder.UseMySql(conn, ServerVersion.AutoDetect(conn));
                return new AppDbContext(optionsBuilder.Options);
            }

            // Default: use SQLite file for design-time migrations
            var builder = new DbContextOptionsBuilder<AppDbContext>();
            var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "rentify_dev.db");
            builder.UseSqlite($"Data Source={dbPath}");
            return new AppDbContext(builder.Options);
        }
    }
}
