using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MyId.Persistence.Data;

namespace MyId.Persistence
{
    /// <summary>
    /// Used by EF Core CLI tools (dotnet ef migrations add/remove) at design time.
    /// Always targets PostgreSQL so generated migrations are Render-compatible.
    /// This class is never called at application runtime.
    /// </summary>
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // Use a placeholder PostgreSQL connection string for migration generation.
            // A real database connection is NOT required for "dotnet ef migrations add".
            optionsBuilder.UseNpgsql(
                "Host=localhost;Port=5432;Database=IdPictureDb_Design;Username=postgres;Password=postgres",
                npgsqlOptions => npgsqlOptions.MigrationsAssembly("MyId.Persistence")
            );

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
