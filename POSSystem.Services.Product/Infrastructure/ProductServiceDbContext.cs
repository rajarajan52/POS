using POSSystem.Domain.Domain.Models;
using Microsoft.EntityFrameworkCore;
using ProductModel = POSSystem.Domain.Domain.Models.Product;

namespace POSSystem.Services.Product.Infrastructure;

/// <summary>
/// Entity Framework Core database context for Product service.
/// Follows Repository pattern for data access abstraction.
/// </summary>
public class ProductServiceDbContext : DbContext
{
    public ProductServiceDbContext(DbContextOptions<ProductServiceDbContext> options)
        : base(options)
    {
    }

    public DbSet<ProductModel> Products => Set<ProductModel>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Product configuration
        modelBuilder.Entity<ProductModel>(builder =>
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Description).HasMaxLength(1000);
            builder.Property(p => p.Sku).IsRequired().HasMaxLength(50);
            builder.Property(p => p.Price).HasPrecision(18, 2);
            builder.HasIndex(p => p.Sku).IsUnique();
        });

        // Category configuration
        modelBuilder.Entity<Category>(builder =>
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Description).HasMaxLength(500);
        });
    }
}


