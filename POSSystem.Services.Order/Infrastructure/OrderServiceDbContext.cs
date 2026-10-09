using POSSystem.Domain.Domain.Models;
using Microsoft.EntityFrameworkCore;
using OrderModel = POSSystem.Domain.Domain.Models.Order;

namespace POSSystem.Services.Order.Infrastructure;

/// <summary>
/// Entity Framework Core database context for Order service.
/// </summary>
public class OrderServiceDbContext : DbContext
{
    public OrderServiceDbContext(DbContextOptions<OrderServiceDbContext> options)
        : base(options)
    {
    }

    public DbSet<OrderModel> Orders => Set<OrderModel>();
    public DbSet<OrderLineItem> OrderLineItems => Set<OrderLineItem>();
    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Order configuration
        modelBuilder.Entity<OrderModel>(builder =>
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.CustomerId).IsRequired();
            builder.Property(o => o.Subtotal).HasPrecision(18, 2);
            builder.Property(o => o.TaxAmount).HasPrecision(18, 2);
            builder.Property(o => o.DiscountAmount).HasPrecision(18, 2);
            builder.Property(o => o.Total).HasPrecision(18, 2);
            builder.Property(o => o.Status).IsRequired();
            builder.HasMany(o => o.LineItems)
                .WithOne()
                .HasForeignKey("OrderId");
        });

        // OrderLineItem configuration
        modelBuilder.Entity<OrderLineItem>(builder =>
        {
            builder.HasKey(li => li.Id);
            builder.Property(li => li.OrderId).IsRequired();
            builder.Property(li => li.ProductId).IsRequired();
            builder.Property(li => li.ProductName).IsRequired().HasMaxLength(200);
            builder.Property(li => li.UnitPrice).HasPrecision(18, 2);
            builder.Property(li => li.Discount).HasPrecision(18, 2);
        });

        // Customer configuration
        modelBuilder.Entity<Customer>(builder =>
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(c => c.LastName).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Email).HasMaxLength(255);
        });
    }
}


