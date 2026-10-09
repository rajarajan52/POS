var builder = DistributedApplication.CreateBuilder(args);

// Add infrastructure  
var cache = builder.AddRedis("cache").WithExternalHttpEndpoints();

// Add microservices (using in-memory databases for development)
var productService = builder.AddProject<Projects.POSSystem_Services_Product>("productservice")
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints();

var inventoryService = builder.AddProject<Projects.POSSystem_Services_Inventory>("inventoryservice")
    .WithHttpHealthCheck("/health")
    .WithReference(productService)
    .WaitFor(productService)
    .WithExternalHttpEndpoints();

var customerService = builder.AddProject<Projects.POSSystem_Services_Customer>("customerservice")
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints();

var orderService = builder.AddProject<Projects.POSSystem_Services_Order>("orderservice")
    .WithHttpHealthCheck("/health")
    .WithReference(inventoryService)
    .WaitFor(inventoryService)
    .WithReference(customerService)
    .WaitFor(customerService)
    .WithExternalHttpEndpoints();

var paymentService = builder.AddProject<Projects.POSSystem_Services_Payment>("paymentservice")
    .WithHttpHealthCheck("/health")
    .WithReference(orderService)
    .WaitFor(orderService)
    .WithExternalHttpEndpoints();

// Add Blazor frontend
builder.AddProject<Projects.POSSystem_UI_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(productService)
    .WaitFor(productService)
    .WithReference(inventoryService)
    .WaitFor(inventoryService)
    .WithReference(orderService)
    .WaitFor(orderService)
    .WithReference(customerService)
    .WaitFor(customerService)
    .WithReference(paymentService)
    .WaitFor(paymentService);

builder.Build().Run();

