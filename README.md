# Warehouse Inventory System

Inventory system for warehouse staff to manage products, record stock movements, scan barcodes, and monitor low-stock products.

## Status

Implemented first backend slice:

- .NET 8 Web API
- PostgreSQL with EF Core migrations
- Product CRUD with search, category filter, and pagination
- Normalized categories and warehouse locations
- Barcode lookup endpoint
- Stock movement endpoint with explicit transaction handling
- Negative-stock validation
- Unique SKU and optional unique barcode constraints
- Repository + Unit of Work structure
- FluentValidation request validation
- Structured stock-change logging
- Optimistic concurrency with a `Version` token
- Angular 18 frontend scaffold
- Docker Compose for PostgreSQL, backend, frontend, and pgAdmin

Frontend feature screens are the next milestone.

## Run With Docker

```bash
cd D:\projectUnosquare
copy .env.example .env
docker compose up --build
```

Services:

- Backend: http://localhost:5000
- Frontend: http://localhost:8080
- Swagger: http://localhost:5000/swagger
- Health: http://localhost:5000/health
- PostgreSQL: localhost:5432
- pgAdmin: http://localhost:5050

The backend applies EF Core migrations on startup when `ApplyMigrations=true`.

## Manual Backend Run

```bash
cd D:\projectUnosquare
dotnet tool restore
dotnet build
dotnet tool run dotnet-ef database update --project backend\WarehouseInventory.Api\WarehouseInventory.Api.csproj --startup-project backend\WarehouseInventory.Api\WarehouseInventory.Api.csproj
dotnet run --project backend\WarehouseInventory.Api\WarehouseInventory.Api.csproj
```

## API Examples

Create product:

```bash
curl -X POST http://localhost:5000/api/products ^
  -H "Content-Type: application/json" ^
  -d "{\"sku\":\"PROD-001\",\"barcode\":\"1234567890\",\"name\":\"Box Cutter\",\"category\":\"Tools\",\"location\":\"A1-B2\",\"price\":9.99,\"reorderThreshold\":50}"
```

List products:

```bash
curl "http://localhost:5000/api/products?search=box&page=1&pageSize=20"
```

Receive stock:

```bash
curl -X POST http://localhost:5000/api/stock/movements ^
  -H "Content-Type: application/json" ^
  -d "{\"productId\":\"PRODUCT_ID_HERE\",\"quantityChange\":100,\"reason\":\"received\",\"createdBy\":\"warehouse1\",\"version\":\"VERSION_FROM_PRODUCT_DETAILS\"}"
```

Ship stock:

```bash
curl -X POST http://localhost:5000/api/stock/movements ^
  -H "Content-Type: application/json" ^
  -d "{\"productId\":\"PRODUCT_ID_HERE\",\"quantityChange\":-25,\"reason\":\"shipped\",\"createdBy\":\"warehouse1\",\"version\":\"VERSION_FROM_PRODUCT_DETAILS\"}"
```

Lookup by barcode:

```bash
curl http://localhost:5000/api/products/barcode/1234567890
```

Low-stock products:

```bash
curl http://localhost:5000/api/stock/alert/low
```

Movement history:

```bash
curl http://localhost:5000/api/stock/movements/PRODUCT_ID_HERE
```

## Concurrency Approach

The data model is normalized so products reference `Categories` and `WarehouseLocations` by foreign key instead of duplicating category/location text on each product. `Products.Sku` has a unique database index, and `Products.Barcode` has a filtered unique index so barcodes remain optional but unique when provided.

Each product has a `Version` GUID configured as an EF Core concurrency token. Product updates and stock movements require the current version sent by the client. When a write succeeds, the API generates a new version. If another request changed the same product first, EF Core raises a concurrency conflict and the API returns `409 Conflict`.

Stock movements are wrapped in an explicit database transaction so updating `Products.CurrentStock` and inserting `StockMovements` commit or roll back together.

## Next Milestone

1. Build product list/add/edit screens.
2. Add scanner page with Enter-key barcode lookup.
3. Add stock movement and low-stock dashboard pages.
4. Add screenshots to this README.
