using POSSystem.Services.Order.Application.Commands;
using POSSystem.Services.Order.Application.CommandHandlers;
using POSSystem.Services.Order.Application.Queries;
using POSSystem.Services.Order.Application.Services;
using POSSystem.Services.Order.Infrastructure;
using POSSystem.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults
builder.AddServiceDefaults();
builder.AddStructuredLogging();
builder.AddAzureKeyVault();

// Add database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=(localdb)\\mssqllocaldb;Database=AspireApp2_Orders;Trusted_Connection=true;";
builder.Services.AddDbContext<OrderServiceDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add POS service configuration
builder.Services.AddPosServiceDefaults(
    typeof(CreateOrderCommand),
    typeof(GetOrderByIdQuery));

// Register repositories
builder.Services.AddScoped(typeof(POSSystem.Domain.Application.Interfaces.IRepository<,>),
    typeof(POSSystem.Services.Order.Infrastructure.EfCoreRepository<>));

// Register domain services
builder.Services.AddScoped<OrderStateTransitionService>();

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

