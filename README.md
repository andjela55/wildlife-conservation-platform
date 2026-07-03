# Wildlife Conservation Platform

Backend MVP for tracking wildlife with GPS collars, ranger reports, and operational alerts.

The current phase is a .NET 8 Web API. Authentication, Angular, Android, and ESP32 integrations are intentionally out of scope for this MVP.

## Backend Stack

- .NET 8 Web API
- Entity Framework Core
- PostgreSQL
- AutoMapper
- Swagger/OpenAPI
- xUnit test project

## Solution Structure

- `WildlifeConservation.Api`
  - Controllers, Swagger, startup, API response DTOs, and response mapping profiles.
  - Response DTOs are grouped by feature, for example `DTOs/Animals/AnimalResponseDto.cs` and `DTOs/Animals/AnimalResponseProfile.cs`.

- `WildlifeConservation.DTOs`
  - Incoming request DTOs only.
  - DTOs are records, for example `CreateAnimalDto`, `UpdateAnimalDto`, and `ResolveAlertDto`.

- `WildlifeConservation.Models`
  - Domain entities grouped by feature.
  - Each entity folder contains the entity, EF Core configuration, and incoming DTO-to-entity AutoMapper profile.

- `WildlifeConservation.Repositories`
  - `WildlifeDbContext`, migrations, entity repositories, and `BaseRepository<TEntity>`.
  - There is no `IBaseRepository`.
  - Each repository interface is declared in the same file as its concrete repository.
  - `BaseRepository<TEntity>` owns shared persistence logic, including no-tracking queries and shallow insert/update/delete operations.

- `WildlifeConservation.Services`
  - Service interfaces and implementations.
  - Services accept incoming DTO records where needed, but return domain models/entities.
  - Controllers map service results to API response DTOs.

- `WildlifeConservation.Shared`
  - Shared enums and common exceptions.

- `WildlifeConservation.Tests`
  - Backend test project.

## Architecture Rules

- Controllers expose API DTOs.
- Services do not return response DTOs.
- Incoming DTOs live in `WildlifeConservation.DTOs`.
- Outgoing response DTOs live in `WildlifeConservation.Api`.
- Repository queries are no-tracking by default.
- Updates use shallow scalar-property updates in `BaseRepository<TEntity>` instead of `DbSet.Update(entity)`.
- EF configurations live beside their entities in the Models project.
- AutoMapper profiles live beside the DTO/entity boundary where they are used.

## Run Locally

Update the PostgreSQL connection string in:

```text
WildlifeConservation.Api/appsettings.json
```

Default connection:

```text
Host=localhost;Port=5432;Database=wildlife_conservation;Username=postgres;Password=postgres
```

Apply migrations:

```powershell
dotnet ef database update --project WildlifeConservation.Repositories\WildlifeConservation.Repositories.csproj --startup-project WildlifeConservation.Repositories\WildlifeConservation.Repositories.csproj
```

To override the design-time migration connection string:

```powershell
$env:WILDLIFE_CONNECTION_STRING="Host=localhost;Port=5432;Database=wildlife_conservation;Username=postgres;Password=your_password"
```

Start the API:

```powershell
dotnet run --project WildlifeConservation.Api\WildlifeConservation.Api.csproj
```

Swagger:

```text
http://localhost:5191/swagger
https://localhost:7246/swagger
```

## Build And Test

```powershell
dotnet build WildlifeConservationPlatform.sln
dotnet test WildlifeConservationPlatform.sln
```

## MVP Domain

- Species
- Subspecies
- Animals
- Collars
- Collar assignment history
- Location points
- Ranger reports
- Alerts
- Users seeded for MVP data ownership, without authentication

## Main Endpoints

- `GET /api/species`
- `GET /api/species/{id}`
- `POST /api/species`
- `GET /api/subspecies`
- `GET /api/subspecies/{id}`
- `POST /api/subspecies`
- `GET /api/animals`
- `GET /api/animals/{id}`
- `POST /api/animals`
- `PUT /api/animals/{id}`
- `GET /api/animals/{id}/locations`
- `GET /api/animals/{id}/reports`
- `GET /api/animals/{id}/alerts`
- `GET /api/collars`
- `GET /api/collars/{id}`
- `POST /api/collars`
- `PUT /api/collars/{id}`
- `POST /api/collar-assignments`
- `PUT /api/collar-assignments/{id}/unassign`
- `POST /api/location-points`
- `GET /api/location-points/latest`
- `GET /api/location-points/by-animal/{animalId}`
- `GET /api/ranger-reports`
- `GET /api/ranger-reports/{id}`
- `POST /api/ranger-reports`
- `GET /api/alerts`
- `GET /api/alerts/{id}`
- `POST /api/alerts`
- `PUT /api/alerts/{id}/resolve`
