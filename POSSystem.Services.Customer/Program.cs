using POSSystem.Services.Customer.Application.Commands;
using POSSystem.Services.Customer.Application.CommandHandlers;
using POSSystem.Services.Customer.Application.Queries;
using POSSystem.Services.Customer.Application.Services;
using POSSystem.Services.Customer.Infrastructure;
using POSSystem.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults
builder.AddServiceDefaults();
builder.AddStructuredLogging();
builder.AddAzureKeyVault();

// Add database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=(localdb)\\mssqllocaldb;Database=AspireApp2_Customers;Trusted_Connection=true;";
builder.Services.AddDbContext<CustomerServiceDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add POS service configuration
builder.Services.AddPosServiceDefaults(
    typeof(CreateCustomerCommand),
    typeof(GetCustomerByIdQuery));

// Register repositories
builder.Services.AddScoped(typeof(POSSystem.Domain.Application.Interfaces.IRepository<,>),
    typeof(POSSystem.Services.Customer.Infrastructure.EfCoreRepository<>));

// Register domain services
builder.Services.AddScoped<POSSystem.Services.Customer.Application.Services.LoyaltyProgramService>();

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

