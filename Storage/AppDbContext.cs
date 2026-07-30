using CraftCalc.Model;
using Microsoft.EntityFrameworkCore;

namespace CraftCalc.Storage
{
    public class AppDbContext : DbContext
    {
        public DbSet<Material> Materials { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<UsedMaterial> UsedMaterials { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string baseDirectory = AppContext.BaseDirectory;
            string dbPath = Path.Combine(baseDirectory, "craftcalc.db");

            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UsedMaterial>()
                .HasOne(um => um.Product)
                .WithMany(p => p.MaterialsUsed)
                .HasForeignKey(um => um.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UsedMaterial>()
                .HasOne(um => um.Material)
                .WithMany()
                .HasForeignKey(um => um.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
