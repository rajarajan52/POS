using POSSystem.Services.Product.Application.Commands;
using POSSystem.Services.Product.Application.Queries;
using POSSystem.Services.Product.Infrastructure;
using POSSystem.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations
builder.AddServiceDefaults();
builder.AddStructuredLogging();
builder.AddAzureKeyVault();

// Add database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=(localdb)\\mssqllocaldb;Database=AspireApp2_Products;Trusted_Connection=true;";
builder.Services.AddDbContext<ProductServiceDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add POS service configuration (CQRS, validators, mappings, etc.)
builder.Services.AddPosServiceDefaults(
    typeof(CreateProductCommand),
    typeof(GetProductByIdQuery));

// Register repositories
builder.Services.AddScoped(typeof(POSSystem.Domain.Application.Interfaces.IRepository<,>), 
    typeof(POSSystem.Services.Product.Infrastructure.EfCoreRepository<>));

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

