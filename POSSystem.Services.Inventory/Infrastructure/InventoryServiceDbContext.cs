using POSSystem.Domain.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace POSSystem.Services.Inventory.Infrastructure;

/// <summary>
/// Entity Framework Core database context for Inventory service.
/// </summary>
public class InventoryServiceDbContext : DbContext
{
    public InventoryServiceDbContext(DbContextOptions<InventoryServiceDbContext> options)
        : base(options)
    {
    }

    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<InventoryItem>(builder =>
        {
            builder.HasKey(i => i.Id);
            builder.Property(i => i.ProductId).IsRequired();
            builder.Property(i => i.QuantityOnHand).IsRequired();
            builder.Property(i => i.ReorderLevel).IsRequired();
            builder.Property(i => i.ReorderQuantity).IsRequired();
            builder.HasIndex(i => i.ProductId).IsUnique();
        });
    }
}


