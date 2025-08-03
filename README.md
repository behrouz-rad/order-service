
[![CI](https://github.com/behrouz-rad/order-service/actions/workflows/ci.yml/badge.svg)](https://github.com/behrouz-rad/order-service/actions/workflows/ci.yml)
# Order Service

## Project Overview

Order Service is a microservice designed to manage customer orders, including order creation, retrieval, and validation. It exposes a RESTful API for order operations and is built with scalability, maintainability, and testability in mind.

## Technology Stack

- **Language:** C# (.NET 9)
- **Frameworks/Libraries:** ASP.NET Core, Entity Framework Core, MediatR, FluentResults, FluentValidation, Serilog (structured logging), Polly
- **Database:** SQL Server (via EF Core)
- **Testing:** xUnit, Moq, FluentAssertions
- **API Documentation:** Swagger/OpenAPI

## Architecture Overview

The project follows a **Domain-Driven Design (DDD)** and **Clean Architecture** approach, organized into distinct layers:

- **API Layer:** Handles HTTP requests, response formatting, and exception handling.
- **Application Layer:** Contains business logic, commands, queries, and DTOs.
- **Domain Layer:** Defines core business entities, value objects, and domain logic.
- **Infrastructure Layer:** Implements data access, repositories, and external service integrations.

**Key Architectural Decisions:**

- **Separation of Concerns:** Each layer has a clear responsibility, improving maintainability and testability.
- **MediatR for CQRS:** Commands and queries are handled via MediatR, decoupling request handling from controllers.
- **FluentResults:** Standardizes operation results and error handling across layers.
- **Exception Handling:** Centralized via custom middleware for consistent API error responses.
- **Database Migrations:** Managed via EF Core with automated migration support at startup.

## Key Components & Modules

- **OrderService.Api:** ASP.NET Core API exposing endpoints for order operations. Handles request validation, error formatting, and API documentation.
- **OrderService.Application:** Implements business logic, command/query handlers, DTOs, and service abstractions.
- **OrderService.Domain:** Contains core domain models (e.g., `Order`, `OrderItem`, value objects) and business rules.
- **OrderService.Infrastructure:** Provides EF Core-based repository implementations, database context, and service registrations.
- **Tests:** Comprehensive unit and integration tests for all layers, ensuring correctness and reliability.

**Interaction Flow:**
1. **API Controller** receives a request and constructs a command/query.
2. **MediatR** dispatches the command/query to the appropriate handler in the Application layer.
3. **Handlers** coordinate domain logic and interact with repositories/services.
4. **Infrastructure** persists/retrieves data via EF Core.
5. **Results** are returned up the stack, with errors handled uniformly.

## Rationale for Architectural Choices

- **DDD & Layered Architecture:** Promotes a clear separation between business logic and infrastructure, making the system easier to evolve and test.
- **CQRS with MediatR:** Decouples read/write operations and simplifies handler testing.
- **FluentResults & Centralized Error Handling:** Ensures consistent error reporting and easier client integration.
- **Serilog Structured Logging:** Structured logs enable advanced querying, filtering, and correlation of log data, making troubleshooting and monitoring more effective, especially in distributed or production environments.
- **EF Core & Automated Migrations:** Simplifies database management.
- **Comprehensive Testing:** Ensures reliability and supports safe refactoring.

## Getting Started

1. **Requirements:** .NET 9 SDK, SQL Server instance.
2. **Setup:**
   - Clone the repository.
   - Configure the connection string in `appsettings.json` or via environment variable (`ConnectionStrings__DefaultConnection`).
3. **Run the Service:**
   ```sh
   dotnet run --project src/OrderService.Api
   ```
   The service will apply any pending database migrations automatically.
4. **API Documentation:** Access Swagger UI at `/swagger` after starting the service.
5. **Testing:**
   - Configure the connection string in `appsettings.Test.json`.
   ```sh
   dotnet test
   ```

---

**Assumptions:**  
- The service is intended to run as part of a larger microservices ecosystem.
- SQL Server is available and accessible for EF Core migrations.
- An empty database with the name specified in the connection string already exists in SQL Server.
- The reader is familiar with .NET development practices.
