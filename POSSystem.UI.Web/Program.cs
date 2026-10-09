using POSSystem.Infrastructure.Configuration;
using POSSystem.UI.Web;
using POSSystem.UI.Web.Components;
using POSSystem.UI.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations
builder.AddServiceDefaults();
builder.AddStructuredLogging();
builder.AddRedisOutputCache("cache");

// Add Razor components
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure HTTP clients for microservices
builder.Services.AddHttpClient<ProductServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:ProductService"] ?? "https+http://productservice");
});

builder.Services.AddHttpClient<InventoryServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:InventoryService"] ?? "https+http://inventoryservice");
});

builder.Services.AddHttpClient<OrderServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:OrderService"] ?? "https+http://orderservice");
});

builder.Services.AddHttpClient<CustomerServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:CustomerService"] ?? "https+http://customerservice");
});

builder.Services.AddHttpClient<PaymentServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:PaymentService"] ?? "https+http://paymentservice");
});

// Add application services
builder.Services.AddScoped<PosCartService>();
builder.Services.AddScoped<PosCheckoutService>();

// Add problem details
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure Permissions-Policy to allow BrowserLink
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Permissions-Policy", "unload=*");
    await next();
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.UseOutputCache();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapDefaultEndpoints();
app.Run();

