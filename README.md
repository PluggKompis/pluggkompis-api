
# Clean Architecture API Boilerplate

A production-ready boilerplate for building scalable ASP.NET Core Web APIs using **Clean Architecture**, **CQRS**, **Mediator Pattern**, and **Entity Framework Core**.

This template provides a clean separation of concerns between layers (API, Application, Domain, Infrastructure), enforcing maintainability, testability, and ease of extension.

---

## 🏁 Getting Started

### **Install & Run**

```bash
git clone https://github.com/mohald-3/Clean-Architecture-API-Boilerplate.git
cd Clean-Architecture-API-Boilerplate
dotnet restore
dotnet run --project API
```

---

### **Database Migration**

```bash
dotnet ef migrations add InitialCreate -p Infrastructure -s API
dotnet ef database update -p Infrastructure -s API
```


## 🏗 Architecture Overview

API → Application → Domain
API → Infrastructure (through DI)
Infrastructure ← Application (Abstractions only)
Domain has zero external dependencies

---

## 🚀 Features

- Clean Architecture structure with clear boundaries
- CQRS with Pipeline Behaviors (Validation, Logging)
- Entity Framework Core (Repository Pattern + Interceptors)
- Authentication & User Context (Dependency Injection ready)
- Global Exception Handling Middleware
- Dependency Injection setup per layer
- Swagger documentation configured
- Generic repository and lightweight Result/PagedResult models

---

### **Layers**
| Layer | Responsibility |
|-------|---------------|
| **Domain** | Core models, entities, value objects, enums |
| **Application** | Business logic, interfaces, DTOs, CQRS handlers, pipeline behaviors |
| **Infrastructure** | Repository implementations, EF Core DbContext, DB config, interceptors, external services |
| **API** | HTTP endpoints, controllers, middleware, DI entry point, Swagger |

---

## 📂 File Structure

```text
CleanArchitecture/
├─ API/
│  ├─ Controllers/
│  │  └─ AuthController.cs
│  ├─ Helpers/
│  │  ├─ AuthenticationSetup.cs
│  │  ├─ SwaggerSetup.cs
│  │  └─ ValidationBehaviorSetup.cs
│  ├─ Middleware/
│  │  └─ ExceptionHandlingMiddleware.cs
│  ├─ appsettings.json
│  └─ Program.cs
│
├─ Application/
│  ├─ Auth/
│  │  └─ Dtos/
│  │     └─ UserDtos.cs
│  ├─ Common/
│  │  ├─ Behaviors/
│  │  │  ├─ LoggingBehavior.cs
│  │  │  └─ ValidationBehavior.cs
│  │  ├─ Interfaces/
│  │  │  ├─ IAuthService.cs
│  │  │  ├─ IGenericRepository.cs
│  │  │  └─ IUserContextService.cs
│  │  └─ Mappings/
│  │     └─ DependencyInjection.cs
│  └─ DependencyInjection.cs
│
├─ Domain/
│  ├─ Common/
│  │  ├─ OperationResult.cs
│  │  └─ PagedResult.cs
│  ├─ Models/
│  │  ├─ Users/
│  │  │  ├─ User.cs
│  │  │  ├─ Role.cs
│  │  │  └─ UserRole.cs
│  │  └─ LogEntry.cs
│
├─ Infrastructure/
│  ├─ Configuration/
│  │  └─ JwtSettings.cs
│  ├─ Database/
│  │  ├─ Seeding/
│  │  ├─ AppDbContext.cs
│  │  └─ AppDbContextFactory.cs
│  ├─ Interceptors/
│  │  └─ LogSaveChangesInterceptor.cs
│  ├─ Repositories/
│  │  ├─ Users/
│  │  └─ GenericRepository.cs
│  ├─ Services/
│  │  ├─ AuthService.cs
│  │  └─ UserContextService.cs
│  └─ DependencyInjection.cs
│
└─ Test/
````

---

## 📦 NuGet Packages Used

| Package                                                              | Purpose                        |
| -------------------------------------------------------------------- | ------------------------------ |
| `Microsoft.EntityFrameworkCore`                                      | ORM                            |
| `Microsoft.EntityFrameworkCore.SqlServer`                            | SQL Server provider            |
| `Microsoft.EntityFrameworkCore.Design`                               | Migrations & scaffolding       |
| `Microsoft.EntityFrameworkCore.Tools`                                | EF CLI tools                   |
| `FluentValidation`                                                   | Request validation             |
| `FluentValidation.DependencyInjectionExtensions`                     | DI integration                 |
| `MediatR` / `MediatR.Extensions.Microsoft.DependencyInjection`       | CQRS mediator pattern          |
| `Swashbuckle.AspNetCore`                                             | Swagger/OpenAPI                |
| `Microsoft.AspNetCore.Authentication.JwtBearer`                      | JWT authentication             |
| `AutoMapper` / `AutoMapper.Extensions.Microsoft.DependencyInjection` | Mapping                        |
| `xUnit`                                                              | Unit testing (in Test project) |

---

## 🔧 Development Guidelines

* Add new use cases under `Application` (CQRS folder per feature recommended)
* Keep **Domain** pure
* Use **interfaces in Application**, implement them in Infrastructure
* Keep controllers thin — delegate work to MediatR handlers
* Use FluentValidation instead of validating inside handlers

