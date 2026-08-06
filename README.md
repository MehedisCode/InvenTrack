# InvenTrack

InvenTrack is an inventory management system API built with .NET 10. It provides JWT authentication, role-based access control, product/category management, supplier management, inventory transactions, purchase tracking, and user administration.

## Architecture

- `src/InvenTrack.API` - ASP.NET Core Web API entrypoint, controllers, middleware, and application wiring.
- `src/InvenTrack.Application` - Application layer with MediatR commands/queries, validation, DTOs, and business logic.
- `src/InvenTrack.Infrastructure` - Infrastructure layer with EF Core persistence, identity services, JWT authentication, repositories, and external integrations.
- `src/InvenTrack.Domain` - Domain entities, enums, and base models.

## Features

- JWT-based authentication and authorization
- Role-based access control: `Admin`, `Manager`, `Staff`
- User management and account administration
- Category and product CRUD operations
- Supplier management and supplier-product assignment
- Inventory stock-in / stock-out transactions
- Purchase records and retrieval
- Swagger/OpenAPI documentation for development
- Serilog logging to console and rolling file logs

## Requirements

- .NET 10 SDK
- PostgreSQL database
- Optional: Visual Studio / VS Code

## Configuration

The API uses `src/InvenTrack.API/appsettings.json` for configuration.

Important settings:

- `ConnectionStrings:DefaultConnection` - PostgreSQL connection string
- `JwtSettings`:
  - `Secret` - JWT signing secret (must be at least 32 bytes)
  - `Issuer`
  - `Audience`
  - `ExpiryMinutes`

Example connection string in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=InvenTrackDb;Username=postgres;Password=postgres"
}
```

## Running Locally

From the repository root:

```bash
cd src/InvenTrack.API
dotnet restore
dotnet build
dotnet run
```

The API will start using the configured Kestrel settings. In development, Swagger is enabled at `/swagger`.

## API Endpoints

### Authentication

- `POST /api/auth/register` - register a new user and receive JWT token
- `POST /api/auth/login` - authenticate and receive JWT token
- `GET /api/auth/me` - get current authenticated user profile

### Products

- `GET /api/products` - list products
- `GET /api/products/{id}` - get product by ID
- `POST /api/products` - create product (`Admin`, `Manager` only)
- `PUT /api/products/{id}` - update product (`Admin`, `Manager` only)
- `DELETE /api/products/{id}` - delete product (`Admin`, `Manager` only)

### Categories

- `GET /api/categories` - list categories
- `POST /api/categories` - create category (`Admin`, `Manager` only)
- `PUT /api/categories/{id}` - update category (`Admin`, `Manager` only)
- `DELETE /api/categories/{id}` - delete category (`Admin`, `Manager` only)

### Suppliers

- `GET /api/suppliers` - list suppliers
- `GET /api/suppliers/search?term={term}` - search suppliers
- `GET /api/suppliers/{id}` - get supplier by ID
- `POST /api/suppliers` - create supplier (`Admin`, `Manager` only)
- `PUT /api/suppliers/{id}` - update supplier (`Admin`, `Manager` only)
- `DELETE /api/suppliers/{id}` - delete supplier (`Admin`, `Manager` only)
- `POST /api/suppliers/{id}/products` - assign products to supplier (`Admin`, `Manager` only)

### Inventory

- `POST /api/inventory/stock-in` - record stock in transaction
- `POST /api/inventory/stock-out` - record stock out transaction
- `GET /api/inventory/history` - list inventory transaction history

### Purchases

- `GET /api/purchases` - list purchases
- `GET /api/purchases/{id}` - get purchase by ID
- `POST /api/purchases` - create purchase

### Users

- `GET /api/users` - list all users (`Admin` only)
- `GET /api/users/{id}` - get user by ID (`Admin` only)
- `POST /api/users` - create user (`Admin` only)
- `PUT /api/users/{id}` - update user (`Admin` only)
- `DELETE /api/users/{id}` - delete user (`Admin` only)

## Notes

- Swagger is typically available only when `ASPNETCORE_ENVIRONMENT` is set to `Development`.
- Ensure the PostgreSQL database exists and is reachable before running.
- The default `JwtSettings:Secret` in sample configuration should be replaced with a strong secret for production.

## Testing

The repository includes unit test projects under `tests/` for the API, application, domain, and infrastructure layers.

To run tests from the repository root:

```bash
dotnet test
```
