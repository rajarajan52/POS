using POSSystem.Domain.Domain.Models;
using Microsoft.EntityFrameworkCore;
using PaymentModel = POSSystem.Domain.Domain.Models.Payment;

namespace POSSystem.Services.Payment.Infrastructure;

/// <summary>
/// Entity Framework Core database context for Payment service.
/// </summary>
public class PaymentServiceDbContext : DbContext
{
    public PaymentServiceDbContext(DbContextOptions<PaymentServiceDbContext> options)
        : base(options)
    {
    }

    public DbSet<PaymentModel> Payments => Set<PaymentModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PaymentModel>(builder =>
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.OrderId).IsRequired();
            builder.Property(p => p.Amount).HasPrecision(18, 2).IsRequired();
            builder.Property(p => p.Method).IsRequired();
            builder.Property(p => p.Status).IsRequired();
            builder.Property(p => p.TransactionReference).HasMaxLength(100);
            builder.Property(p => p.ReceiptNumber).HasMaxLength(50);
            builder.HasIndex(p => p.TransactionReference);
        });
    }
}


