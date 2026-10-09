using POSSystem.Domain.Application.DTOs;
using POSSystem.Services.Payment.Application.Commands;
using POSSystem.Services.Payment.Application.Services;
using POSSystem.Services.Payment.Infrastructure;
using POSSystem.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults
builder.AddServiceDefaults();
builder.AddStructuredLogging();
builder.AddAzureKeyVault();

// Add database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=(localdb)\\mssqllocaldb;Database=AspireApp2_Payments;Trusted_Connection=true;";
builder.Services.AddDbContext<PaymentServiceDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add POS service configuration
builder.Services.AddPosServiceDefaults(typeof(ProcessPaymentCommand));

// Register repositories
builder.Services.AddScoped(typeof(POSSystem.Domain.Application.Interfaces.IRepository<,>),
    typeof(POSSystem.Services.Payment.Infrastructure.EfCoreRepository<>));

// Register domain services
builder.Services.AddScoped<POSSystem.Services.Payment.Application.Interfaces.IPaymentGateway, POSSystem.Services.Payment.Infrastructure.MockPaymentGateway>();
builder.Services.AddScoped<POSSystem.Services.Payment.Application.Services.ReceiptGenerationService>();

// Add API controllers
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
}

app.UsePosApplicationPipeline();
app.MapControllers();
app.MapDefaultEndpoints();
app.Run();

