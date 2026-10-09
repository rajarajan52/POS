# AspireApp2 POS System - Project Flow Map

## 📋 Project Overview
A distributed microservices-based Point of Sale (POS) system built with .NET 10, using Aspire for orchestration and Blazor for the frontend. The system follows CQRS (Command Query Responsibility Segregation) pattern with MediatR for command/query handling.

---

## 🏗️ System Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                     BLAZOR UI FRONTEND                          │
│              POSSystem.UI.Web (Interactive Server)              │
│                                                                  │
│  ┌──────────────┬──────────────┬──────────────────────────────┐ │
│  │ ProductCatalog  ShoppingCart  PosHome (Dashboard)           │ │
│  │   Page          Page           Page                         │ │
│  └──────────────┴──────────────┴──────────────────────────────┘ │
│                                                                  │
│  Service Clients:                                               │
│  • ProductServiceClient  • OrderServiceClient                   │
│  • PaymentServiceClient  • InventoryServiceClient               │
│  • CustomerServiceClient • PosCheckoutService (Orchestrator)    │
└─────────────────────────────────────────────────────────────────┘
								│
					┌───────────┼───────────┐
					│           │           │
		 ┌──────────▼──┐   ┌────▼────┐   ┌─▼─────────┐
		 │   PRODUCT   │   │  ORDER  │   │ INVENTORY │
		 │   SERVICE   │   │ SERVICE │   │  SERVICE  │
		 └────────────┘    └────────┘    └──────────┘
					│           │           │
		 ┌──────────▼──┐   ┌────▼────┐   ┌─▼──────────┐
		 │   PAYMENT   │   │ CUSTOMER│   │ ORCHESTRATION
		 │   SERVICE   │   │ SERVICE │   │   (AppHost)
		 └────────────┘    └────────┘    └────────────┘
```

---

## 📦 Microservices Breakdown

### 1️⃣ PRODUCT SERVICE
**Responsibility**: Product catalog management, browsing, and product information

```
ProductServiceClient (UI)
		│
		▼
ProductsController
		│
	┌───┴───┐
	│       │
	▼       ▼
GET /api/products          POST /api/products
(GetAllProductsQuery)      (CreateProductCommand)
	│                           │
	▼                           ▼
GetAllProductsQueryHandler  CreateProductCommandHandler
	│                           │
	└───────────┬───────────────┘
				▼
		 ProductRepository
				▼
		 Product Domain Model
				▼
		 EF Core DbContext
```

**Key Components:**
- **Commands**: CreateProductCommand
- **Queries**: GetAllProductsQuery
- **Handlers**: CreateProductCommandHandler, GetAllProductsQueryHandler
- **Domain Model**: Product aggregate
- **DTO**: ProductDto
- **Storage**: SQL Database via EF Core

**Data Flow:**
```
Create Product:
UI Request → CreateProductCommand → MediatR Pipeline → Validation → 
Handler → Repository → Database → Response DTO

Browse Products:
UI Request → GetAllProductsQuery → Handler → Repository → 
Database → List<ProductDto> → UI Display
```

---

### 2️⃣ ORDER SERVICE
**Responsibility**: Order creation, processing, and order state management

```
OrderServiceClient (UI)
		│
		▼
OrdersController
		│
	┌───┴───┐
	│       │
	▼       ▼
GET /api/orders/{id}       POST /api/orders
(GetOrderByIdQuery)        (CreateOrderCommand)
	│                           │
	▼                           ▼
GetOrderByIdQueryHandler    CreateOrderCommandHandler
	│                           │
	│                       ┌───┴────────┐
	│                       │            │
	│                       ▼            ▼
	│                   Validate    Check Customer
	│                   Line Items  (CustomerService)
	│                       │
	│                       ▼
	│                Create Order
	│                       │
	│                       ▼
	│            OrderStateTransitionService
	│            (State: Pending → Confirmed → 
	│             Processing → Completed)
	│                       │
	└───────────┬───────────┘
				▼
		 OrderRepository
				▼
		 Order Domain Model
				▼
		 EF Core DbContext
```

**Key Components:**
- **Commands**: CreateOrderCommand
- **Queries**: GetOrderByIdQuery
- **Domain Model**: Order aggregate with LineItems collection
- **State Machine**: OrderStateTransitionService
- **DTO**: OrderDto, OrderLineItemDto
- **Integrations**: Calls CustomerService to validate customer

**State Transitions:**
```
Pending → Confirmed → Processing → Completed
   ↓                                    
Cancelled (from any state)
```

---

### 3️⃣ PAYMENT SERVICE
**Responsibility**: Payment processing, receipt generation, and transaction management

```
PaymentServiceClient (UI)
		│
		▼
PaymentsController
		│
		▼
POST /api/payments/process
(ProcessPaymentCommand)
		│
		▼
ProcessPaymentCommandHandler
		│
	┌───┴──────────┐
	│              │
	▼              ▼
IPaymentGateway   ReceiptGenerationService
(MockPaymentGateway)
	│                  │
	▼                  ▼
Process Payment    Generate Receipt
(External Call)         │
	│                   ▼
	└───────┬───────────┘
			▼
	 PaymentRepository
			▼
	 Payment Domain Model
			▼
	 EF Core DbContext
```

**Key Components:**
- **Commands**: ProcessPaymentCommand
- **Gateway**: IPaymentGateway (MockPaymentGateway for dev)
- **Services**: ReceiptGenerationService
- **Domain Model**: Payment aggregate
- **DTO**: PaymentDto
- **Methods Supported**: Cash, CreditCard, DebitCard, MobileWallet

**Payment Flow:**
```
Order Created → Payment Request → Gateway Processing → 
Transaction ID Generated → Receipt Created → Payment Stored → 
Response with Receipt Number
```

---

### 4️⃣ INVENTORY SERVICE
**Responsibility**: Stock management, availability checking, and inventory reservations

```
InventoryServiceClient (UI)
		│
		▼
InventoryController
		│
	┌───┴────────────────┐
	│                    │
	▼                    ▼
GET /api/inventory/     POST /api/inventory/
check-availability      reserve-stock
	│                    │
	▼                    ▼
CheckInventoryAvailability  ReserveStockCommand
QueryHandler                    │
	│                           ▼
	│                   ReserveStockCommandHandler
	│                           │
	│                       ┌───┴──────┐
	│                       │          │
	│                       ▼          ▼
	│                   Update Stock   Emit Event
	│                       │          (OrderCreated)
	│                       ▼
	└───────┬────────InventoryRepository
			▼
	InventoryItem Model
			▼
	EF Core DbContext
```

**Key Components:**
- **Commands**: ReserveStockCommand
- **Queries**: CheckInventoryAvailabilityQuery
- **Handlers**: CheckInventoryAvailabilityQueryHandler, ReserveStockCommandHandler
- **Events**: ProductOrderedEventHandler (listens for order events)
- **Domain Model**: InventoryItem
- **Storage**: SQL Database

**Inventory Workflow:**
```
Check Availability:
Product ID + Quantity → Query → Check Stock Level → 
Return Boolean (Available/Not Available)

Reserve Stock:
Order Created → Event Fired → Event Handler → Reserve Stock → 
Decrement Available Quantity → Update Database
```

---

### 5️⃣ CUSTOMER SERVICE
**Responsibility**: Customer profile management and loyalty program

```
CustomerServiceClient (UI)
		│
		▼
CustomersController
		│
	┌───┴──────────┐
	│              │
	▼              ▼
GET /api/customers/   POST /api/customers
{id}                  (CreateCustomerCommand)
	│                  │
	▼                  ▼
GetCustomerByIdQuery  CreateCustomerCommand
	│                  │
	│                  ▼
	│            CreateCustomerCommandHandler
	│                  │
	│             ┌────┴────┐
	│             │         │
	│             ▼         ▼
	│         Validate    Generate
	│         Unique Email  Loyalty ID
	│             │
	└───┬─────────┘
		▼
CustomerRepository
		▼
Customer Domain Model
(with LoyaltyPoints)
		▼
EF Core DbContext

┌──────────────────────────┐
│ LoyaltyProgramService    │
│ • AddPoints()            │
│ • RedeemPoints()         │
│ • GetLoyaltyBalance()    │
└──────────────────────────┘
```

**Key Components:**
- **Commands**: CreateCustomerCommand, UpdateCustomerCommand, AddLoyaltyPointsCommand
- **Queries**: GetCustomerByIdQuery
- **Services**: LoyaltyProgramService
- **Domain Model**: Customer aggregate
- **DTO**: CustomerDto
- **Features**: Email uniqueness, Loyalty points tracking

**Customer Workflow:**
```
Create Customer:
UI Input → CreateCustomerCommand → Validate Email Unique → 
Create Customer with Loyalty ID → Store → Return DTO

Loyalty Program:
Purchase Completed → Add Points → Update Balance → 
Can Be Redeemed on Future Purchases
```

---

## 🔄 Cross-Service Interactions

### Checkout Flow (Complete Transaction)
```
┌─────────────────────────────────────────────────────────────┐
│                   BLAZOR UI                                 │
│              (ShoppingCart.razor)                            │
└─────────────────────────────────────────────────────────────┘
		│
		▼
┌─────────────────────────────────────────────────────────────┐
│           PosCheckoutService (Orchestrator)                 │
│                  (UI Layer)                                  │
└─────────────────────────────────────────────────────────────┘
		│
	┌───┼───┬────────┐
	│   │   │        │
	▼   ▼   ▼        ▼
	1. Check Inventory (InventoryService)
			│ IsAvailable?
			▼
	2. Create Order (OrderService)
			│ Order Created
			▼
	3. Process Payment (PaymentService)
			│ Payment Processed
			▼
	4. Generate Receipt
			│ Receipt Number
			▼
	5. Clear Cart & Redirect
			│
			▼
		Success ✅
```

**Detailed Checkout Sequence:**
```
User Clicks "Complete Purchase"
	│
	▼
For Each Cart Item:
	1. InventoryServiceClient.CheckAvailabilityAsync(productId, quantity)
	   → POST to /api/inventory/check-availability
	   → Returns: IsAvailable (bool)
	│
	▼
If All Items Available:
	2. OrderServiceClient.CreateOrderAsync(orderDto)
	   → POST to /api/orders
	   → Calls OrderService → Creates order in database
	   → Returns: OrderDto with ID
	│
	▼
	3. PaymentServiceClient.ProcessPaymentAsync(orderId, amount, method)
	   → POST to /api/payments/process
	   → Calls PaymentService → Gateway processes payment
	   → Returns: PaymentDto with ReceiptNumber
	│
	▼
Success:
	4. CartService.Clear()
	5. Navigation.NavigateTo($"/receipt/{receiptNumber}")

Failure at any step:
	→ Return CheckoutResult with error message
	→ Display error to user
	→ Keep items in cart
```

---

## 📊 Data Models & DTOs

### Domain Models
```
├── Product
│   ├── Id: Guid
│   ├── Name: string
│   ├── Description: string
│   ├── Price: decimal
│   ├── Sku: string
│   ├── CategoryId: Guid
│   └── CreatedAt: DateTime
│
├── Order
│   ├── Id: Guid
│   ├── CustomerId: Guid
│   ├── Status: OrderStatus (Pending, Confirmed, Processing, Completed, Cancelled)
│   ├── LineItems: List<OrderLineItem>
│   ├── Subtotal: decimal
│   ├── TaxAmount: decimal
│   ├── DiscountAmount: decimal
│   ├── Total: decimal
│   ├── CreatedAt: DateTime
│   └── CompletedAt: DateTime?
│
├── OrderLineItem
│   ├── ProductId: Guid
│   ├── ProductName: string
│   ├── UnitPrice: decimal
│   ├── Quantity: int
│   └── Discount: decimal
│
├── Payment
│   ├── Id: Guid
│   ├── OrderId: Guid
│   ├── Amount: decimal
│   ├── Method: PaymentMethod (Cash, CreditCard, DebitCard, MobileWallet)
│   ├── Status: PaymentStatus (Pending, Completed, Failed, Refunded)
│   ├── TransactionId: string
│   ├── ProcessedAt: DateTime
│   └── ReceiptNumber: string
│
├── Customer
│   ├── Id: Guid
│   ├── Name: string
│   ├── Email: string (Unique)
│   ├── Phone: string
│   ├── LoyaltyId: string
│   ├── LoyaltyPoints: int
│   └── CreatedAt: DateTime
│
└── InventoryItem
	├── Id: Guid
	├── ProductId: Guid
	├── QuantityAvailable: int
	├── QuantityReserved: int
	└── LastUpdated: DateTime
```

---

## 🌐 API Endpoints Summary

### Product Service
```
GET    /api/products              → Get all products
GET    /api/products/{id}         → Get product by ID
POST   /api/products              → Create new product
PUT    /api/products/{id}         → Update product
DELETE /api/products/{id}         → Delete product
```

### Order Service
```
GET    /api/orders/{id}           → Get order by ID
POST   /api/orders                → Create new order
PUT    /api/orders/{id}/status    → Update order status
GET    /api/orders                → Get orders (paged)
```

### Payment Service
```
POST   /api/payments/process      → Process payment
GET    /api/payments/{id}         → Get payment details
POST   /api/payments/{id}/refund  → Refund payment
```

### Inventory Service
```
POST   /api/inventory/check-availability  → Check if items in stock
POST   /api/inventory/reserve-stock       → Reserve stock for order
GET    /api/inventory/{productId}         → Get inventory level
PUT    /api/inventory/{productId}         → Update inventory
```

### Customer Service
```
GET    /api/customers/{id}        → Get customer by ID
POST   /api/customers             → Create new customer
PUT    /api/customers/{id}        → Update customer
POST   /api/customers/{id}/loyalty/add    → Add loyalty points
GET    /api/customers/{id}/loyalty/balance → Get loyalty balance
```

---

## 🛠️ Technology Stack

| Layer | Technology |
|-------|-----------|
| **Frontend** | Blazor Web (Interactive Server) |
| **API Gateway** | Aspire (Service Discovery & Orchestration) |
| **Backend** | ASP.NET Core Minimal APIs / MVC Controllers |
| **Command/Query Bus** | MediatR with Validation & Logging Behaviors |
| **ORM** | Entity Framework Core |
| **Database** | SQL Server |
| **Caching** | Redis (Output Caching) |
| **Logging** | Serilog + Structured Logging |
| **Validation** | FluentValidation |
| **AutoMapper** | DTO Mapping |
| **Testing** | xUnit, Moq, FluentAssertions |

---

## 📁 Project Structure

```
AspireApp2/
├── POSSystem.Domain/                    # Shared domain models & DTOs
│   ├── Domain/Models/                   # Aggregates
│   │   ├── Product.cs
│   │   ├── Order.cs
│   │   ├── Payment.cs
│   │   ├── Customer.cs
│   │   └── InventoryItem.cs
│   ├── Application/
│   │   ├── DTOs/                        # Data Transfer Objects
│   │   ├── Interfaces/                  # Repository & Handler interfaces
│   │   └── Exceptions/                  # Domain exceptions
│   └── POSSystem.Domain.csproj
│
├── POSSystem.Infrastructure/            # Shared infrastructure
│   ├── Configuration/                   # DI & service setup
│   ├── Behaviors/                       # MediatR pipeline behaviors
│   ├── AzureKeyVaultConfigurationExtensions.cs
│   └── POSSystem.Infrastructure.csproj
│
├── POSSystem.Services.Product/          # Product Service
│   ├── Program.cs
│   ├── Controllers/ProductsController.cs
│   ├── Application/
│   │   ├── Commands/
│   │   ├── Queries/
│   │   ├── Validators/
│   │   └── Handlers/
│   ├── Infrastructure/
│   │   ├── ProductServiceDbContext.cs
│   │   └── EfCoreRepository.cs
│   └── POSSystem.Services.Product.csproj
│
├── POSSystem.Services.Order/            # Order Service
│   ├── Program.cs
│   ├── Controllers/OrdersController.cs
│   ├── Application/
│   │   ├── Commands/
│   │   ├── Queries/
│   │   ├── Services/OrderStateTransitionService.cs
│   │   └── Handlers/
│   ├── Infrastructure/
│   │   └── OrderServiceDbContext.cs
│   └── POSSystem.Services.Order.csproj
│
├── POSSystem.Services.Payment/          # Payment Service
│   ├── Program.cs
│   ├── Controllers/PaymentsController.cs
│   ├── Application/
│   │   ├── Commands/
│   │   ├── Services/ReceiptGenerationService.cs
│   │   ├── Interfaces/IPaymentGateway.cs
│   │   └── Handlers/
│   ├── Infrastructure/
│   │   ├── PaymentServiceDbContext.cs
│   │   └── MockPaymentGateway.cs
│   └── POSSystem.Services.Payment.csproj
│
├── POSSystem.Services.Inventory/        # Inventory Service
│   ├── Program.cs
│   ├── Controllers/InventoryController.cs
│   ├── Application/
│   │   ├── Commands/
│   │   ├── Queries/
│   │   ├── EventHandlers/
│   │   └── Handlers/
│   ├── Infrastructure/
│   │   └── InventoryServiceDbContext.cs
│   └── POSSystem.Services.Inventory.csproj
│
├── POSSystem.Services.Customer/         # Customer Service
│   ├── Program.cs
│   ├── Controllers/CustomersController.cs
│   ├── Application/
│   │   ├── Commands/
│   │   ├── Services/LoyaltyProgramService.cs
│   │   ├── Validators/
│   │   └── Handlers/
│   ├── Infrastructure/
│   │   └── CustomerServiceDbContext.cs
│   └── POSSystem.Services.Customer.csproj
│
├── POSSystem.Orchestration/             # Aspire AppHost
│   ├── AppHost.cs
│   ├── appsettings.json
│   └── POSSystem.Orchestration.csproj
│
├── POSSystem.UI.Web/                    # Blazor Web Frontend
│   ├── Program.cs
│   ├── Components/
│   │   ├── Pages/
│   │   │   ├── PosHome.razor           # Main dashboard
│   │   │   ├── ProductCatalog.razor    # Browse products
│   │   │   ├── ShoppingCart.razor      # Cart & checkout
│   │   │   ├── Counter.razor
│   │   │   └── Weather.razor
│   │   ├── Layout/
│   │   │   ├── MainLayout.razor
│   │   │   └── NavMenu.razor
│   │   └── App.razor
│   ├── Services/
│   │   ├── ProductServiceClient.cs
│   │   ├── OrderServiceClient.cs
│   │   ├── PaymentServiceClient.cs
│   │   ├── InventoryServiceClient.cs
│   │   ├── CustomerServiceClient.cs
│   │   ├── PosCartService.cs            # Client-side cart state
│   │   └── PosCheckoutService.cs        # Checkout orchestration
│   ├── wwwroot/
│   │   └── [static assets]
│   └── POSSystem.UI.Web.csproj
│
├── POSSystem.Services.Product.Tests/    # Product Service Tests
│   ├── Application/
│   ├── Domain/
│   ├── Infrastructure/
│   └── POSSystem.Services.Product.Tests.csproj
│
└── AspireApp2.slnx                      # Solution file

```

---

## 🔐 Security & Validation

### Request Validation Pipeline
```
Request
   │
   ▼
Authorization (antiforgery for state-changing operations)
   │
   ▼
MediatR Pipeline
   │
   ├─→ ValidationBehavior
   │   └─→ FluentValidation rules
   │       └─→ ValidationException if invalid
   │
   ├─→ LoggingBehavior
   │   └─→ Structured logging
   │
   └─→ Command/Query Handler
	   └─→ Business Logic
		   │
		   └─→ Response/Exception
```

---

## 🚀 Request/Response Lifecycle

### Example: Create Order (Happy Path)
```
1. USER ACTION
   User clicks "Complete Purchase" on ShoppingCart.razor

2. CLIENT-SIDE
   ShoppingCart.razor calls CheckoutService.CheckoutAsync()

3. ORCHESTRATION (PosCheckoutService)
   a) For each item: await InventoryServiceClient.CheckAvailabilityAsync()
	  → POST /api/inventory/check-availability
	  → InventoryService validates stock
	  → Returns: IsAvailable (bool)

   b) Create order: await OrderServiceClient.CreateOrderAsync()
	  → POST /api/orders
	  → OrderService receives OrderDto
	  → MediatR routes to CreateOrderCommand
	  → Validation behavior validates command
	  → CreateOrderCommandHandler executes
	  → Repository saves Order to database
	  → Returns: OrderDto with ID

   c) Process payment: await PaymentServiceClient.ProcessPaymentAsync()
	  → POST /api/payments/process
	  → PaymentService receives payment request
	  → MediatR routes to ProcessPaymentCommand
	  → ProcessPaymentCommandHandler calls IPaymentGateway
	  → MockPaymentGateway returns success
	  → ReceiptGenerationService creates receipt
	  → Repository saves Payment to database
	  → Returns: PaymentDto with ReceiptNumber

   d) Return CheckoutResult with Success=true

4. CLIENT-SIDE (continued)
   CartService.Clear()
   Navigation.NavigateTo($"/receipt/{receiptNumber}")

5. USER SEES
   Receipt page with order confirmation
```

---

## ⚠️ Error Handling Strategy

```
Layer                 Error Handling
─────────────────────────────────────
Blazor UI            → Try-catch → CheckoutResult
					 → Display error message
					 → Keep cart items

Service Client       → Timeout (30s)
					 → Non-success HTTP response
					 → JSON deserialization failure
					 → Returns null

Service Handler      → Validation failed
					 → Business logic error
					 → Database error
					 → Logged + Exception thrown

Database             → Constraint violations
					 → Connection failures
					 → Transaction rollbacks

Response Path        ← Exception handled
					 ← Error message created
					 ← HTTP error response
					 ← Client receives null/error
					 ← User sees error message
```

---

## 📈 Scaling Considerations

### Current Bottlenecks
- Single database per service (could be optimized with event sourcing)
- Synchronous HTTP calls between services (could use message queues)
- No caching layer beyond Redis output caching
- No request deduplication for idempotent operations

### Scalability Improvements
1. **Add Message Queue (RabbitMQ/Azure Service Bus)**
   - Decouple payment processing from order service
   - Enable asynchronous inventory updates

2. **Implement Event Sourcing**
   - Audit trail of all state changes
   - Better consistency guarantees

3. **Add API Gateway (Kong/Ocelot)**
   - Rate limiting
   - Request aggregation
   - Centralized authentication

4. **Cache Strategy**
   - Product catalog (rarely changes)
   - Customer loyalty balance
   - Order history

---

## 🧪 Testing Strategy

```
Unit Tests
├── Domain Model Tests (OrderTests, ProductTests)
├── Validator Tests (ProductValidatorTests)
└── Handler Tests (CreateProductCommandHandlerTests)

Integration Tests
├── Repository Tests (EfCoreRepositoryTests)
├── DbContext Tests
└── Full command/query flow

E2E Tests
├── Checkout flow
├── Payment processing
└── Multi-service interactions
```

---

## 🎯 Key Takeaways

1. **Microservices Pattern**: Each service has its own database and API
2. **CQRS Pattern**: Commands modify state, Queries retrieve state
3. **Orchestration**: PosCheckoutService coordinates multi-service transaction
4. **Resilience**: Timeouts, null handling, graceful degradation
5. **DDD**: Domain models with business logic (not anemic)
6. **Validation**: Three layers - domain, service, and API
7. **Observability**: Structured logging + MediatR behaviors
8. **Type Safety**: DTOs prevent coupling between services

---

## 📞 Service Communication Matrix

| From → To | Protocol | Async | Timeout |
|-----------|----------|-------|---------|
| UI → Product | HTTP | ✓ | 30s |
| UI → Order | HTTP | ✓ | 30s |
| UI → Payment | HTTP | ✓ | 30s |
| UI → Inventory | HTTP | ✓ | 30s |
| UI → Customer | HTTP | ✓ | 30s |
| Order → Customer | HTTP | ✓ | 30s |
| Order → Inventory | Event | ✓ | N/A |
| Payment → Receipt Gen | In-Process | ✓ | N/A |
| Inventory → Event Bus | Event | ✓ | N/A |

---

**Generated**: 2026  
**Version**: 1.0  
**Framework**: .NET 10 with Aspire + Blazor
