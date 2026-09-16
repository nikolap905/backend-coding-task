# Insurance Claims API

A .NET 9 Web API for managing insurance claims, policy covers, and automated premium calculations with audit logging.

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- Docker Desktop

## Running locally

The app uses [Testcontainers](https://dotnet.testcontainers.org/) to spin up disposable SQL Server and MongoDB containers automatically on startup — no manual database setup required.

```bash
cd Claims
dotnet run
```

This opens Swagger UI (`/swagger`) in your browser once the app is listening. First run will be slower while Docker pulls the SQL Server and MongoDB images.

## Architecture

- Controllers/ — Handles HTTP requests, routing, and status codes.
- Services/ — Contains core business logic (PremiumCalculator, claim/cover workflows).
- Models/ — Domain entities and data contracts.
- Data/ — MongoDB context (ClaimsContext) for core entity persistence.
- Auditing/ — SQL Server context (AuditContext) for immutable event logs.

