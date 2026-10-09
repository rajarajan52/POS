using POSSystem.Services.Inventory.Application.Commands;
using POSSystem.Services.Inventory.Application.EventHandlers;
using POSSystem.Services.Inventory.Application.Queries;
using POSSystem.Services.Inventory.Infrastructure;
using POSSystem.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults
builder.AddServiceDefaults();
builder.AddStructuredLogging();
builder.AddAzureKeyVault();

// Add database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=(localdb)\\mssqllocaldb;Database=AspireApp2_Inventory;Trusted_Connection=true;";
builder.Services.AddDbContext<InventoryServiceDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add POS service configuration
builder.Services.AddPosServiceDefaults(
    typeof(ReserveStockCommand),
    typeof(CheckInventoryAvailabilityQuery));

// Register repositories
builder.Services.AddScoped(typeof(POSSystem.Domain.Application.Interfaces.IRepository<,>),
    typeof(POSSystem.Services.Inventory.Infrastructure.EfCoreRepository<>));

// Register event handlers
builder.Services.AddScoped<StockReservedEventHandler>();
builder.Services.AddScoped<InsufficientStockEventHandler>();

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

