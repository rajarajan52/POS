# Deployment Guide - Enterprise POS System

This guide provides step-by-step instructions for deploying the POS System to various environments.

## Table of Contents
- [Local Development](#local-development)
- [Docker Deployment](#docker-deployment)
- [IIS Deployment](#iis-deployment)
- [Azure Deployment](#azure-deployment)
- [Kubernetes Deployment](#kubernetes-deployment)

---

## Local Development

### Prerequisites
- .NET 10 SDK
- Visual Studio 2026 or VS Code
- SQL Server 2019+ or LocalDB
- Redis (optional, for caching)

### Setup Steps

#### 1. Clone Repository
```bash
git clone https://github.com/rajarajan52/POS.git
cd POS
```

#### 2. Create Databases
The project uses Entity Framework Core migrations. Databases are created automatically on first run.

```bash
# In Package Manager Console (Visual Studio)
Update-Database -Context ProductServiceDbContext
Update-Database -Context OrderServiceDbContext
Update-Database -Context PaymentServiceDbContext
Update-Database -Context InventoryServiceDbContext
Update-Database -Context CustomerServiceDbContext
```

Or using CLI:
```bash
dotnet ef database update --project POSSystem.Services.Product
dotnet ef database update --project POSSystem.Services.Order
dotnet ef database update --project POSSystem.Services.Payment
dotnet ef database update --project POSSystem.Services.Inventory
dotnet ef database update --project POSSystem.Services.Customer
```

#### 3. Configure appsettings.json

Update connection strings in each service's `appsettings.json`:

**POSSystem.Services.Product/appsettings.json**
```json
{
  "ConnectionStrings": {
	"ProductDb": "Server=.;Database=POSSystem_Product;Integrated Security=true;TrustServerCertificate=true;"
  },
  "Services": {
	"ProductService": "http://localhost:5001"
  }
}
```

Repeat for other services (Order, Payment, Inventory, Customer) with appropriate ports and connection strings.

#### 4. Run Aspire Orchestration
```bash
cd POSSystem.Orchestration
dotnet run
```

This will:
- Start the Aspire Dashboard (http://localhost:15000)
- Launch all microservices
- Start the Blazor UI (http://localhost:7004)

#### 5. Access the Application
- **UI**: http://localhost:7004
- **Aspire Dashboard**: http://localhost:15000
- **Product Service**: http://localhost:5001
- **Order Service**: http://localhost:5002
- **Payment Service**: http://localhost:5003
- **Inventory Service**: http://localhost:5004
- **Customer Service**: http://localhost:5005

---

## Docker Deployment

### Build Docker Images

#### Option 1: Build Individual Services

**Product Service**
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 as build
WORKDIR /src
COPY ["POSSystem.Services.Product/POSSystem.Services.Product.csproj", "POSSystem.Services.Product/"]
COPY ["POSSystem.Domain/POSSystem.Domain.csproj", "POSSystem.Domain/"]
COPY ["POSSystem.Infrastructure/POSSystem.Infrastructure.csproj", "POSSystem.Infrastructure/"]
RUN dotnet restore "POSSystem.Services.Product/POSSystem.Services.Product.csproj"

COPY . .
RUN dotnet build "POSSystem.Services.Product/POSSystem.Services.Product.csproj" -c Release -o /app/build

FROM build as publish
RUN dotnet publish "POSSystem.Services.Product/POSSystem.Services.Product.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 as runtime
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5001
ENTRYPOINT ["dotnet", "POSSystem.Services.Product.dll"]
```

Build the image:
```bash
docker build -f POSSystem.Services.Product.Dockerfile -t pos-product-service:latest .
```

#### Option 2: Use Docker Compose

Create `docker-compose.yml`:
```yaml
version: '3.8'

services:
  sqlserver:
	image: mcr.microsoft.com/mssql/server:2022-latest
	environment:
	  ACCEPT_EULA: Y
	  SA_PASSWORD: YourStrong@Password123
	ports:
	  - "1433:1433"
	volumes:
	  - mssql_data:/var/opt/mssql

  redis:
	image: redis:latest
	ports:
	  - "6379:6379"

  product-service:
	build:
	  context: .
	  dockerfile: POSSystem.Services.Product.Dockerfile
	environment:
	  ConnectionStrings__ProductDb: "Server=sqlserver;Database=POSSystem_Product;User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=true;"
	ports:
	  - "5001:5001"
	depends_on:
	  - sqlserver

  order-service:
	build:
	  context: .
	  dockerfile: POSSystem.Services.Order.Dockerfile
	environment:
	  ConnectionStrings__OrderDb: "Server=sqlserver;Database=POSSystem_Order;User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=true;"
	ports:
	  - "5002:5002"
	depends_on:
	  - sqlserver

  payment-service:
	build:
	  context: .
	  dockerfile: POSSystem.Services.Payment.Dockerfile
	environment:
	  ConnectionStrings__PaymentDb: "Server=sqlserver;Database=POSSystem_Payment;User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=true;"
	ports:
	  - "5003:5003"
	depends_on:
	  - sqlserver

  inventory-service:
	build:
	  context: .
	  dockerfile: POSSystem.Services.Inventory.Dockerfile
	environment:
	  ConnectionStrings__InventoryDb: "Server=sqlserver;Database=POSSystem_Inventory;User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=true;"
	ports:
	  - "5004:5004"
	depends_on:
	  - sqlserver

  customer-service:
	build:
	  context: .
	  dockerfile: POSSystem.Services.Customer.Dockerfile
	environment:
	  ConnectionStrings__CustomerDb: "Server=sqlserver;Database=POSSystem_Customer;User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=true;"
	ports:
	  - "5005:5005"
	depends_on:
	  - sqlserver

  ui-web:
	build:
	  context: .
	  dockerfile: POSSystem.UI.Web.Dockerfile
	environment:
	  Services__ProductService: "http://product-service:5001"
	  Services__OrderService: "http://order-service:5002"
	  Services__PaymentService: "http://payment-service:5003"
	  Services__InventoryService: "http://inventory-service:5004"
	  Services__CustomerService: "http://customer-service:5005"
	ports:
	  - "7004:7004"
	depends_on:
	  - product-service
	  - order-service
	  - payment-service
	  - inventory-service
	  - customer-service

volumes:
  mssql_data:
```

#### Run Docker Compose
```bash
docker-compose up -d
```

Access the application at `http://localhost:7004`

---

## IIS Deployment

### Prerequisites
- IIS 10.0 or higher
- .NET 10 Hosting Bundle
- SQL Server database

### Deployment Steps

#### 1. Publish Applications
```bash
# For each service
dotnet publish -c Release -o ./publish/ProductService POSSystem.Services.Product
dotnet publish -c Release -o ./publish/OrderService POSSystem.Services.Order
dotnet publish -c Release -o ./publish/PaymentService POSSystem.Services.Payment
dotnet publish -c Release -o ./publish/InventoryService POSSystem.Services.Inventory
dotnet publish -c Release -o ./publish/CustomerService POSSystem.Services.Customer
dotnet publish -c Release -o ./publish/UIWeb POSSystem.UI.Web
```

#### 2. Create IIS Sites
1. Open IIS Manager
2. Create a new Application Pool for each service (.NET CLR version: No Managed Code)
3. Create websites:
   - Name: `POS-Product`, Port: 5001, Path: `publish/ProductService`
   - Name: `POS-Order`, Port: 5002, Path: `publish/OrderService`
   - Name: `POS-Payment`, Port: 5003, Path: `publish/PaymentService`
   - Name: `POS-Inventory`, Port: 5004, Path: `publish/InventoryService`
   - Name: `POS-Customer`, Port: 5005, Path: `publish/CustomerService`
   - Name: `POS-UI`, Port: 7004, Path: `publish/UIWeb`

#### 3. Configure web.config
Create `web.config` in each publish directory:
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <location path="." inheritInChildApplications="false">
	<system.webServer>
	  <handlers>
		<add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
	  </handlers>
	  <aspNetCore processPath="dotnet" arguments=".\POSSystem.Services.Product.dll" stdoutLogEnabled="false" stdoutLogFile=".\logs\stdout" hostingModel="inprocess" />
	</system.webServer>
  </location>
</configuration>
```

---

## Azure Deployment

### Option 1: Azure App Service

#### 1. Create Azure Resources
```bash
# Using Azure CLI
az group create --name POS-RG --location eastus

# Create App Service Plan
az appservice plan create --name POS-Plan --resource-group POS-RG --sku B2 --is-linux

# Create Web App
az webapp create --resource-group POS-RG --plan POS-Plan --name pos-ui-web --runtime "DOTNET|10"

# Create SQL Database
az sql server create --resource-group POS-RG --name pos-db-server --admin-user sqladmin --admin-password YourStrong@Password123
az sql db create --resource-group POS-RG --server pos-db-server --name POSSystem_Product
```

#### 2. Publish to Azure
```bash
# Using Visual Studio
# Right-click project → Publish → Choose Azure App Service
# Select target and publish

# Or using CLI
dotnet publish -c Release
az webapp up --resource-group POS-RG --name pos-ui-web --plan POS-Plan --sku B2
```

#### 3. Configure App Settings
In Azure Portal → App Service → Configuration:
```
ConnectionStrings__ProductDb = Server=tcp:pos-db-server.database.windows.net,1433;Initial Catalog=POSSystem_Product;Persist Security Info=False;User ID=sqladmin;Password=YourStrong@Password123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
Services__ProductService = https://pos-product-service.azurewebsites.net
```

### Option 2: Azure Container Instances

```bash
# Build and push Docker image
docker build -t pos-product-service:latest -f POSSystem.Services.Product.Dockerfile .
docker tag pos-product-service:latest <registry>.azurecr.io/pos-product-service:latest
docker push <registry>.azurecr.io/pos-product-service:latest

# Deploy to ACI
az container create \
  --resource-group POS-RG \
  --name pos-product-service \
  --image <registry>.azurecr.io/pos-product-service:latest \
  --ports 5001 \
  --registry-login-server <registry>.azurecr.io \
  --registry-username <username> \
  --registry-password <password>
```

---

## Kubernetes Deployment

### Prerequisites
- kubectl
- Helm 3.0+
- Azure Kubernetes Service (AKS) or similar

### Create Helm Chart

**Chart.yaml**
```yaml
apiVersion: v2
name: pos-system
description: A Helm chart for POS System
type: application
version: 1.0.0
appVersion: "1.0"
```

**values.yaml**
```yaml
replicaCount: 2

productService:
  image: <registry>/pos-product-service:latest
  port: 5001

orderService:
  image: <registry>/pos-order-service:latest
  port: 5002

paymentService:
  image: <registry>/pos-payment-service:latest
  port: 5003

inventoryService:
  image: <registry>/pos-inventory-service:latest
  port: 5004

customerService:
  image: <registry>/pos-customer-service:latest
  port: 5005

uiWeb:
  image: <registry>/pos-ui-web:latest
  port: 7004

sqlServer:
  image: mcr.microsoft.com/mssql/server:2022-latest
  password: YourStrong@Password123

redis:
  image: redis:latest
```

### Deploy to Kubernetes

```bash
# Install Helm chart
helm install pos-system ./helm-chart --namespace default

# Check deployment status
kubectl get pods
kubectl get svc

# View logs
kubectl logs -f deployment/pos-product-service

# Scale service
kubectl scale deployment/pos-product-service --replicas=3
```

---

## Monitoring & Logging

### Application Insights (Azure)
```csharp
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddApplicationInsightsKubernetesEnricher();
```

### Logging Configuration
```json
{
  "Serilog": {
	"MinimumLevel": "Information",
	"WriteTo": [
	  {
		"Name": "Console"
	  },
	  {
		"Name": "File",
		"Args": {
		  "path": "logs/log-.txt",
		  "rollingInterval": "Day"
		}
	  }
	]
  }
}
```

---

## Health Checks

### Add Health Check Endpoints
```csharp
builder.Services.AddHealthChecks()
	.AddDbContextCheck<ProductServiceDbContext>()
	.AddUrlGroup(new Uri("http://localhost:5002/health"), name: "Order Service");

app.MapHealthChecks("/health");
```

---

## Rollback Procedures

### Docker
```bash
# Revert to previous image
docker-compose up -d --no-build
docker run -d previous-service:tag
```

### Azure App Service
```bash
# Via Azure Portal: Deployment slots → Swap
# Via CLI
az webapp deployment slot swap --resource-group POS-RG --name pos-ui-web --slot staging
```

### Kubernetes
```bash
# Rollback deployment
kubectl rollout undo deployment/pos-product-service
kubectl rollout history deployment/pos-product-service
```

---

## Performance Optimization

### Database Optimization
- Enable query caching
- Add indexes on frequently searched columns
- Use connection pooling

### Application Optimization
- Enable output caching for product catalog
- Use compression for API responses
- Implement request deduplication

### Infrastructure Optimization
- Use CDN for static assets
- Enable auto-scaling based on CPU/memory
- Use load balancing across multiple instances

---

## Disaster Recovery

### Backup Strategy
```bash
# SQL Server backup
BACKUP DATABASE POSSystem_Product 
TO DISK = '/var/opt/mssql/backup/product_backup.bak'

# Automated backup (Azure)
az sql db backup create --resource-group POS-RG --server pos-db-server --name POSSystem_Product
```

### Recovery Procedures
1. Restore database from backup
2. Redeploy application code
3. Run health checks
4. Monitor logs for errors

---

For additional help, refer to:
- [README.md](README.md) - Project overview
- [PROJECT_FLOW_MAP.md](PROJECT_FLOW_MAP.md) - Architecture details
- Official .NET Documentation: https://docs.microsoft.com/dotnet
