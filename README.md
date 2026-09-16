# Insurance Claims API

A backend API for managing insurance claims: create, read and delete claims, and their related covers (policies). Cover premiums are computed automatically based on vessel type and insurance period length. Every create/delete is recorded in an audit trail.

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- Docker Desktop (or another Docker daemon) — running, before you start the app

## Running locally

The app uses [Testcontainers](https://dotnet.testcontainers.org/) to spin up disposable SQL Server and MongoDB containers automatically on startup — no manual database setup required.

```bash
cd Claims
dotnet run
```

This opens Swagger UI (`/swagger`) in your browser once the app is listening. First run will be slower while Docker pulls the SQL Server and MongoDB images.

## Architecture

- **Controllers/** — Handles HTTP requests and responses.
- **Services/** — Contains claim and cover operations and premium calculation.
- **Models/** — Domain entities used by the application and persistence layer.
- **Data/** — Database context and persistence configuration.
- **Auditing/** — Handles recording create and delete operations in the audit database.

## API

- `GET /Claims`, `GET /Claims/{id}`, `POST /Claims`, `DELETE /Claims/{id}`
- `GET /Covers`, `GET /Covers/{id}`, `POST /Covers`, `DELETE /Covers/{id}`
- `POST /Covers/compute` — computes a premium for a given start/end date and cover type without persisting anything

See `docs/README.md` for the original take-home assignment brief.
