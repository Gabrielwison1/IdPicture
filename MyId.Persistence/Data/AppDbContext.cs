using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyId.Domain.Entities;

namespace MyId.Persistence.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Education> Educations { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<Skills> Skills { get; set; }
        public DbSet<GalleryItem> GalleryItems { get; set; }
        public DbSet<TaxCalculator> TaxCalculators { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<TaxCalculator>(entity =>
            {
                entity.Property(t => t.GrossIncome).HasPrecision(18, 2);
                entity.Property(t => t.TaxableIncome).HasPrecision(18, 2);
                entity.Property(t => t.EstimatedTax).HasPrecision(18, 2);
            });
        }
    }
}