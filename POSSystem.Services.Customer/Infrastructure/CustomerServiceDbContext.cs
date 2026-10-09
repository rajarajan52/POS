using POSSystem.Domain.Domain.Models;
using Microsoft.EntityFrameworkCore;
using CustomerModel = POSSystem.Domain.Domain.Models.Customer;

namespace POSSystem.Services.Customer.Infrastructure;

/// <summary>
/// Entity Framework Core database context for Customer service.
/// </summary>
public class CustomerServiceDbContext : DbContext
{
    public CustomerServiceDbContext(DbContextOptions<CustomerServiceDbContext> options)
        : base(options)
    {
    }

    public DbSet<CustomerModel> Customers => Set<CustomerModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CustomerModel>(builder =>
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(c => c.LastName).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Email).HasMaxLength(255);
            builder.Property(c => c.PhoneNumber).HasMaxLength(20);
            builder.Property(c => c.LoyaltyPoints).HasPrecision(18, 2);
            builder.HasIndex(c => c.Email).IsUnique();
        });
    }
}


