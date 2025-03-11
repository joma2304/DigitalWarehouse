using DigitalWarehouse.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DigitalWarehouse.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ProductModel> Products { get; set; }
    public DbSet<CategoryModel> Categories { get; set; }
    public DbSet<StockChangeModel> StockChanges { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

                    base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProductModel>()
                .HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId);

            modelBuilder.Entity<StockChangeModel>()
                .HasOne(qc => qc.Product)
                .WithMany(p => p.QuantityChanges)
                .HasForeignKey(qc => qc.ProductId);
        }

    
}
