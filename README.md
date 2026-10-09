# Enterprise POS System

A distributed microservices-based Point of Sale (POS) system built with **.NET 10**, **Aspire for orchestration**, and **Blazor Web** for the frontend. The system follows modern architecture patterns including **CQRS**, **Domain-Driven Design (DDD)**, and **Microservices**.

## 🎯 Project Overview

This is a fully-functional, production-ready POS system designed to demonstrate enterprise-level development practices with .NET 10. It manages product catalogs, orders, payments, inventory, and customer loyalty programs across multiple microservices.

### Key Features
✅ **Multi-tenant Microservices Architecture** - 5 independent services with their own databases  
✅ **Blazor Web Frontend** - Interactive server-side rendering with real-time updates  
✅ **CQRS Pattern** - Clear separation of commands and queries using MediatR  
✅ **Domain-Driven Design** - Rich domain models with business logic  
✅ **Event-Driven** - Asynchronous event handling for inter-service communication  
✅ **Comprehensive Validation** - FluentValidation across all layers  
✅ **Structured Logging** - Serilog with Aspire integration  
✅ **Full Test Coverage** - Unit and integration tests with xUnit  

---

## 🏗️ Architecture

### System Diagram
```
┌─────────────────────────────────────────────────────────────────┐
│                     BLAZOR UI FRONTEND                          │
│              POSSystem.UI.Web (Interactive Server)              │
└─────────────────────────────────────────────────────────────────┘
								│
			┌───────────────────┼───────────────────┐
			│                   │                   │
	┌───────▼────────┐  ┌───────▼─────┐  ┌────────▼───────┐
	│   PRODUCT      │  │    ORDER    │  │   INVENTORY    │
	│   SERVICE      │  │   SERVICE   │  │   SERVICE      │
	└────────────────┘  └─────────────┘  └────────────────┘
			│                   │                   │
	┌───────▼────────┐  ┌───────▼─────┐  ┌────────▼───────┐
	│   PAYMENT      │  │  CUSTOMER   │  │ ORCHESTRATION  │
	│   SERVICE      │  │  SERVICE    │  │   (AppHost)    │
	└────────────────┘  └─────────────┘  └────────────────┘
```

### Microservices

| Service | Port | Responsibility |
|---------|------|-----------------|
| **Product Service** | 5001 | Product catalog & browsing |
| **Order Service** | 5002 | Order creation & processing |
| **Payment Service** | 5003 | Payment processing & receipts |
| **Inventory Service** | 5004 | Stock management & reservations |
| **Customer Service** | 5005 | Customer profiles & loyalty program |
| **Blazor UI** | 7004 | Web frontend |
| **Orchestration (AppHost)** | N/A | Service discovery & routing |

---

## 🚀 Getting Started

### Prerequisites
- **.NET 10 SDK** or later
- **Visual Studio 2026** Community or higher (or VS Code)
- **SQL Server** (LocalDB included with VS)
- **Redis** (for output caching)
- **Git**

### Quick Start

#### 1. Clone the Repository
```bash
git clone https://github.com/rajarajan52/POS.git
cd POS
```

#### 2. Build the Solution
```bash
dotnet build
```

#### 3. Run with Aspire Dashboard
```bash
cd POSSystem.Orchestration
dotnet run
```

The Aspire Dashboard will open at `http://localhost:15000` showing all services.

#### 4. Access the Application
- **Blazor UI**: http://localhost:7004
- **API Services**: Routed through Aspire

---

## 📁 Project Structure

```
POS/
├── POSSystem.Domain/                    # Shared domain models & DTOs
│   ├── Domain/Models/                   # Aggregates
│   │   ├── Product.cs
│   │   ├── Order.cs
│   │   ├── Payment.cs
│   │   ├── Customer.cs
│   │   └── InventoryItem.cs
│   └── Application/
│       ├── DTOs/                        # Data Transfer Objects
│       ├── Interfaces/                  # Repository & Handler interfaces
│       └── Exceptions/                  # Domain exceptions
│
├── POSSystem.Infrastructure/            # Shared infrastructure
│   ├── Configuration/ServiceConfiguration.cs
│   ├── Behaviors/                       # MediatR pipeline behaviors
│   └── AzureKeyVaultConfigurationExtensions.cs
│
├── POSSystem.Services.Product/          # Product Microservice
│   ├── Controllers/
│   ├── Application/
│   │   ├── Commands/
│   │   ├── Queries/
│   │   ├── Validators/
│   │   └── Handlers/
│   └── Infrastructure/
│
├── POSSystem.Services.Order/            # Order Microservice
│   ├── Controllers/
│   ├── Application/
│   ├── Services/OrderStateTransitionService.cs
│   └── Infrastructure/
│
├── POSSystem.Services.Payment/          # Payment Microservice
│   ├── Controllers/
│   ├── Application/
│   ├── Services/ReceiptGenerationService.cs
│   ├── Interfaces/IPaymentGateway.cs
│   └── Infrastructure/MockPaymentGateway.cs
│
├── POSSystem.Services.Inventory/        # Inventory Microservice
│   ├── Controllers/
│   ├── Application/
│   └── EventHandlers/
│
├── POSSystem.Services.Customer/         # Customer Microservice
│   ├── Controllers/
│   ├── Application/
│   └── Services/LoyaltyProgramService.cs
│
├── POSSystem.Orchestration/             # Aspire AppHost
│   ├── AppHost.cs
│   └── appsettings.json
│
├── POSSystem.UI.Web/                    # Blazor Web Frontend
│   ├── Components/
│   │   ├── Pages/
│   │   │   ├── PosHome.razor           # Dashboard
│   │   │   ├── ProductCatalog.razor    # Browse products
│   │   │   └── ShoppingCart.razor      # Checkout
│   │   └── Layout/
│   ├── Services/
│   │   ├── ProductServiceClient.cs
│   │   ├── OrderServiceClient.cs
│   │   ├── PaymentServiceClient.cs
│   │   ├── InventoryServiceClient.cs
│   │   ├── CustomerServiceClient.cs
│   │   ├── PosCartService.cs            # Client-side cart
│   │   └── PosCheckoutService.cs        # Checkout orchestration
│   └── wwwroot/
│
└── POSSystem.Services.Product.Tests/    # Unit & Integration Tests
	├── Application/
	├── Domain/
	└── Infrastructure/
```

---

## 💻 Key Technologies

| Layer | Technology | Version |
|-------|-----------|---------|
| **Framework** | .NET | 10.0 |
| **Frontend** | Blazor Web (Interactive Server) | 10.0 |
| **Orchestration** | Aspire | 13.5+ |
| **CQRS/Mediator** | MediatR | 12.4+ |
| **ORM** | Entity Framework Core | 10.0 |
| **Validation** | FluentValidation | 11.9+ |
| **Mapping** | AutoMapper | 15.1+ |
| **Logging** | Serilog | 4.1+ |
| **Caching** | Redis (Aspire) | Via Aspire |
| **Testing** | xUnit + Moq + FluentAssertions | Latest |

---

## 🎯 API Endpoints

### Product Service
```
GET    /api/products              → List all products
GET    /api/products/{id}         → Get product details
POST   /api/products              → Create product
PUT    /api/products/{id}         → Update product
DELETE /api/products/{id}         → Delete product
```

### Order Service
```
GET    /api/orders/{id}           → Get order details
POST   /api/orders                → Create order
PUT    /api/orders/{id}/status    → Update order status
GET    /api/orders                → List orders (paged)
```

### Payment Service
```
POST   /api/payments/process      → Process payment
GET    /api/payments/{id}         → Get payment details
POST   /api/payments/{id}/refund  → Refund payment
```

### Inventory Service
```
POST   /api/inventory/check-availability    → Check stock
POST   /api/inventory/reserve-stock         → Reserve items
GET    /api/inventory/{productId}           → Get inventory level
PUT    /api/inventory/{productId}           → Update stock
```

### Customer Service
```
GET    /api/customers/{id}        → Get customer
POST   /api/customers             → Create customer
PUT    /api/customers/{id}        → Update customer
POST   /api/customers/{id}/loyalty/add      → Add loyalty points
GET    /api/customers/{id}/loyalty/balance → Get loyalty balance
```

---

## 🔄 Checkout Flow Example

```
User adds items to cart
		↓
User clicks "Complete Purchase"
		↓
CheckoutService.CheckoutAsync()
		├→ InventoryService.CheckAvailabilityAsync()
		│   └→ Verify all items in stock
		├→ OrderService.CreateOrderAsync()
		│   └→ Create order with line items
		├→ PaymentService.ProcessPaymentAsync()
		│   └→ Process payment & generate receipt
		└→ Return CheckoutResult
				↓
Success?
├→ YES: Clear cart, show receipt
└→ NO: Display error, keep items in cart
```

---

## 🧪 Testing

### Run All Tests
```bash
dotnet test
```

### Run Specific Test Project
```bash
dotnet test POSSystem.Services.Product.Tests
```

### Test Coverage
```bash
dotnet test /p:CollectCoverage=true
```

### Test Projects
- **POSSystem.Services.Product.Tests** - Product service tests
- Unit tests for commands, queries, validators
- Integration tests for repository & database

---

## 📊 Data Models

### Product
```csharp
public class Product : AggregateRoot
{
	public string Name { get; set; }
	public string Description { get; set; }
	public decimal Price { get; set; }
	public string Sku { get; set; }
	public Guid CategoryId { get; set; }
	public DateTime CreatedAt { get; set; }
}
```

### Order
```csharp
public class Order : AggregateRoot
{
	public Guid CustomerId { get; set; }
	public OrderStatus Status { get; set; }
	public List<OrderLineItem> LineItems { get; set; }
	public decimal Subtotal { get; set; }
	public decimal TaxAmount { get; set; }
	public decimal Total { get; set; }
	public DateTime CreatedAt { get; set; }
}
```

### Payment
```csharp
public class Payment : AggregateRoot
{
	public Guid OrderId { get; set; }
	public decimal Amount { get; set; }
	public PaymentMethod Method { get; set; }
	public PaymentStatus Status { get; set; }
	public string TransactionId { get; set; }
	public string ReceiptNumber { get; set; }
	public DateTime ProcessedAt { get; set; }
}
```

---

## 🔐 Security Features

- **Input Validation**: FluentValidation on all inputs
- **Antiforgery**: Built-in ASP.NET Core protection
- **Permissions Policy**: Configured for BrowserLink compatibility
- **Error Handling**: Comprehensive error handling at all layers
- **Timeout Protection**: 30-second timeouts on all HTTP calls
- **Structured Logging**: Audit trail for all operations

---

## 📈 Performance Features

- **Output Caching**: Redis for product catalog caching
- **Async/Await**: All I/O operations are async
- **Connection Pooling**: EF Core connection pooling enabled
- **Request Validation Pipeline**: Early validation reduces processing
- **Graceful Degradation**: Service failures don't block entire system

---

## 🚀 Deployment

### Docker Support
```bash
# Build Docker image
docker build -t pos-system .

# Run container
docker run -p 7004:7004 pos-system
```

### Azure Deployment
The system is configured for Azure Key Vault integration:
```csharp
builder.AddAzureKeyVault(credentials, vaultUri, new AzureKeyVaultConfigurationProvider());
```

### Environment Configuration
```json
{
  "Services": {
	"ProductService": "https+http://productservice",
	"OrderService": "https+http://orderservice",
	"PaymentService": "https+http://paymentservice",
	"InventoryService": "https+http://inventoryservice",
	"CustomerService": "https+http://customerservice"
  }
}
```

---

## 📚 Documentation

- **[PROJECT_FLOW_MAP.md](PROJECT_FLOW_MAP.md)** - Comprehensive architecture and flow diagrams
- **[API Documentation](docs/API.md)** - Detailed API endpoint documentation
- **[Database Schema](docs/DATABASE.md)** - Entity relationships and migrations

---

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Code Guidelines
- Follow C# naming conventions (PascalCase for public members)
- Use async/await for all I/O operations
- Add XML documentation comments for public APIs
- Keep methods focused and single-responsibility
- Write unit tests for new features

---

## 📋 Project Status

| Component | Status | Version |
|-----------|--------|---------|
| Core Architecture | ✅ Complete | 1.0 |
| Microservices | ✅ Complete | 1.0 |
| Blazor Frontend | ✅ Complete | 1.0 |
| API Endpoints | ✅ Complete | 1.0 |
| Unit Tests | ✅ Complete | 1.0 |
| Documentation | ✅ Complete | 1.0 |
| Docker Support | ⏳ Planned | TBD |
| Kubernetes Deployment | ⏳ Planned | TBD |

---

## 📞 Support & Issues

- **Report Bugs**: Use GitHub Issues
- **Feature Requests**: Discussions section
- **Documentation Questions**: Check docs folder first

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🙏 Acknowledgments

- Built with .NET 10 and Aspire
- UI powered by Blazor Web
- Architecture inspired by DDD and CQRS patterns
- Community contributions and feedback

---

## 📞 Contact

**Author**: Rajarajan  
**Email**: rajarajan52@gmail.com  
**GitHub**: [@rajarajan52](https://github.com/rajarajan52)

---

**Happy Coding! 🚀**
