# ERP Inventory Management API

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?style=flat&logo=postgresql&logoColor=white)](https://postgresql.org)
[![Redis](https://img.shields.io/badge/Redis-7.0-DC382D?style=flat&logo=redis&logoColor=white)](https://redis.io)
[![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?style=flat&logo=docker&logoColor=white)](https://docker.com)
[![CI](https://img.shields.io/github/actions/workflow/status/faizkhan005/erp-inventory-api/ci.yml?label=CI&style=flat&logo=github-actions)](https://github.com/faizkhan005/erp-inventory-api/actions)
[![License](https://img.shields.io/badge/License-MIT-22C55E?style=flat)](LICENSE)

A **production-grade RESTful API** for enterprise inventory management, built with Clean Architecture, Redis caching, and JWT authentication. Inspired by real-world ERP systems I worked on at Epicor Software.

> 🎯 **Why this project?** Enterprise ERP systems need APIs that are fast, secure, and maintainable at scale. This project demonstrates exactly those principles using a tech stack that maps directly to production .NET backend roles.

---

## ✨ Features

- **Clean Architecture** — strict separation of API / Application / Domain / Infrastructure layers
- **Full CRUD** for Products, Categories, and Warehouses with proper validation
- **JWT Authentication** — register, login, refresh tokens, role-based authorization
- **Redis Caching** — cache-aside pattern on all GET endpoints, automatic invalidation on writes
- **Pagination** — cursor-based pagination with configurable page size
- **Filtering & Sorting** — filter by category, warehouse, price range; sort by any field
- **Swagger / OpenAPI** — fully documented interactive API at `/swagger`
- **Docker Compose** — one command to spin up API + PostgreSQL + Redis
- **GitHub Actions CI** — builds and runs tests on every push to `main`
- **Azure Deployment** — live at [https://erp-api.azurewebsites.net](https://erp-api.azurewebsites.net) *(coming soon)*

---

## 🏗️ Architecture

```
ErpInventoryApi/
├── ErpInventoryApi.API/            # Controllers, middleware, DI config
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── ProductsController.cs
│   │   ├── CategoriesController.cs
│   │   └── WarehousesController.cs
│   ├── Middleware/
│   │   ├── ExceptionHandlingMiddleware.cs
│   │   └── RequestLoggingMiddleware.cs
│   └── Program.cs
│
├── ErpInventoryApi.Application/    # Business logic, interfaces, DTOs
│   ├── Interfaces/
│   │   ├── IProductService.cs
│   │   └── ICacheService.cs
│   ├── Services/
│   │   └── ProductService.cs
│   └── DTOs/
│       ├── ProductDto.cs
│       └── PagedResultDto.cs
│
├── ErpInventoryApi.Domain/         # Entities, value objects (no dependencies)
│   ├── Entities/
│   │   ├── Product.cs
│   │   ├── Category.cs
│   │   └── Warehouse.cs
│   └── Common/
│       └── BaseEntity.cs
│
└── ErpInventoryApi.Infrastructure/ # EF Core, Redis, external services
    ├── Data/
    │   ├── AppDbContext.cs
    │   └── Migrations/
    ├── Repositories/
    │   └── ProductRepository.cs
    └── Services/
        └── RedisCacheService.cs
```

**Key design decisions:**
- Domain layer has **zero external dependencies** — pure C# classes only
- All database access goes through **Repository pattern** with interfaces in Application
- **Cache-aside pattern**: check cache → if miss, hit DB → populate cache → return
- Redis keys follow the pattern: `product:{id}` and `products:page:{n}:size:{s}`

---

## 🚀 Quick Start

### Prerequisites
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (recommended)
- OR: [.NET 10 SDK](https://dotnet.microsoft.com/download) + PostgreSQL 16 + Redis 7

### Run with Docker (Recommended)

```bash
# Clone the repo
git clone https://github.com/faizkhan005/erp-inventory-api.git
cd erp-inventory-api

# Start all services (API + PostgreSQL + Redis)
docker-compose up --build

# API is now running at http://localhost:5000
# Swagger UI at http://localhost:5000/swagger
```

### Run Locally (without Docker)

```bash
# 1. Update connection strings in appsettings.Development.json
# 2. Run database migrations
dotnet ef database update --project ErpInventoryApi.Infrastructure

# 3. Run the API
dotnet run --project ErpInventoryApi.API
```

---

## 📡 API Endpoints

### Authentication
| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/auth/register` | Register a new user |
| `POST` | `/api/auth/login` | Login and receive JWT token |
| `POST` | `/api/auth/refresh` | Refresh expired access token |

### Products
| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/products` | Get all products (paginated, filterable) |
| `GET` | `/api/products/{id}` | Get product by ID *(Redis cached)* |
| `POST` | `/api/products` | Create new product *(Auth required)* |
| `PUT` | `/api/products/{id}` | Update product *(Auth required)* |
| `DELETE` | `/api/products/{id}` | Delete product *(Admin only)* |

### Query Parameters (GET /api/products)
```
?page=1&pageSize=10          # Pagination
&categoryId={guid}           # Filter by category
&warehouseId={guid}          # Filter by warehouse
&minPrice=10&maxPrice=500    # Price range filter
&sortBy=name&sortDir=asc     # Sort by any field
&search=laptop               # Full-text search on name/SKU
```

### Example Response
```json
{
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "Industrial Pump Model A",
      "sku": "PUMP-IND-001",
      "price": 1249.99,
      "stockQuantity": 47,
      "category": { "id": "...", "name": "Pumps" },
      "warehouse": { "id": "...", "name": "Cleveland Warehouse" },
      "createdAt": "2025-01-20T08:00:00Z"
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 10,
    "totalCount": 247,
    "totalPages": 25,
    "hasNext": true,
    "hasPrevious": false
  }
}
```

---

## ⚙️ Tech Stack

| Layer | Technology | Why |
|-------|------------|-----|
| Runtime | .NET 10 | Latest LTS, best performance |
| Web Framework | ASP.NET Core 10 | Industry standard for .NET APIs |
| ORM | Entity Framework Core 10 | Type-safe DB access, migrations |
| Database | PostgreSQL 16 | Production-grade, open source |
| Caching | Redis 7 | Sub-millisecond reads, widely used in .NET backends |
| Authentication | JWT + ASP.NET Identity | Stateless, scalable auth |
| Container | Docker + Docker Compose | Reproducible environments |
| CI | GitHub Actions | Automated build + test |
| Docs | Swagger / OpenAPI | Auto-generated, interactive |

---

## 🧪 Running Tests

```bash
dotnet test
```

Test coverage includes:
- Unit tests for all service methods
- Integration tests for all API endpoints (using `WebApplicationFactory`)
- Repository tests with an in-memory database

---

## 🌱 What I Learned Building This

1. **Cache invalidation strategy matters.** A naive "cache everything" approach causes stale data. I implemented explicit invalidation on write operations so the cache stays consistent with PostgreSQL.

2. **Clean Architecture pays off immediately.** When I added Redis, I only touched the Infrastructure layer — no changes to Domain or Application. That's the point.

3. **Pagination should be cursor-based for large datasets.** Offset pagination (`SKIP n TAKE m`) gets expensive at high offsets. Cursor-based pagination using the last seen `id` is O(log n).

4. **Docker Compose made environment parity a non-issue.** No more "works on my machine" — the Docker Compose file is the environment definition.

---

## 🗺️ Roadmap

- [x] Clean Architecture setup
- [x] EF Core + PostgreSQL (Products, Categories, Warehouses)
- [x] JWT Authentication
- [x] Redis Caching (cache-aside)
- [x] Pagination + filtering + sorting
- [x] Swagger docs
- [x] Docker Compose
- [x] GitHub Actions CI
- [ ] Azure App Service deployment
- [ ] Unit + integration test suite
- [ ] Health check endpoint (`/health`)
- [ ] Rate limiting middleware
- [ ] Structured logging with Serilog

---

## 👤 Author

**Faizan** — .NET Full Stack Developer  
2+ years at Epicor Software building enterprise ERP and mobile applications.

- 💼 [LinkedIn](https://www.linkedin.com/in/faizan-814521191)
- 📧 [Email](mailto:faizanahmedkhan005@gmail.com)
- 🐙 [GitHub](https://github.com/faizkhan005)

---

## 📄 License

MIT — see [LICENSE](LICENSE) for details.
