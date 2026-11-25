# MovieApi

ASP.NET Core Web API for managing movies, actors, reviews, and aggregate reports. The project uses Entity Framework Core with SQL Server and exposes OpenAPI and a Scalar API reference in the Development environment.

## Technologies

- .NET 10 / ASP.NET Core
- Entity Framework Core 10
- SQL Server
- OpenAPI (`Microsoft.AspNetCore.OpenApi`)
- Scalar API reference
- Controller-based ASP.NET Core APIs
- Fluent EF Core entity configurations
- Repository and unit-of-work pattern
- Request/response records with mapping extensions
- Automatic migrations, seed data, and global exception handling

## Requirements

- .NET 10 SDK
- SQL Server or SQL Server Express
- A reachable SQL Server instance configured through `ConnectionStrings:MovieDb`

The default connection in `appsettings.json` is:

```text
Server=localhost;Database=MovieDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true
```

Update `MovieApi/appsettings.Development.json` or environment configuration if SQL Server is hosted elsewhere.

## Run the application

From the repository root:

```bash
dotnet restore MovieApi.slnx
cd MovieApi
dotnet run
```

On startup, the application:

1. Registers controllers and dependencies.
2. Registers Problem Details and the global exception handler.
3. Creates the database context.
4. Applies pending EF Core migrations asynchronously; EF Core runs the configured async seed callback.
5. Seeds initial data only when no movies exist.
6. Registers HTTPS redirection and status-code pages.
7. Maps controllers and, in Development, `/openapi/v1.json` and the Scalar API reference.

## API base route

All routes use `/api` as the base prefix.

## Actors

### `GET /api/actors`

Returns all actors ordered by name.

**Response:** `200 OK`

```json
[
  {
    "id": 1,
    "name": "Tim Robbins",
    "birthYear": 1958,
    "role": null
  }
]
```

`role` is omitted from JSON output when it is null.

### `GET /api/actors/{id}`

Returns an actor by integer ID.

- `200 OK` — actor found
- `404 Not Found` — actor does not exist

### `POST /api/actors`

Creates an actor.

```json
{
  "name": "Margot Robbie",
  "birthYear": 1990
}
```

- `201 Created` — returns the created `ActorResponse`
- `400 Bad Request` — request validation fails

### `PUT /api/actors/{id}`

Updates an existing actor.

```json
{
  "name": "Margot Elise Robbie",
  "birthYear": 1990
}
```

- `204 No Content` — actor updated
- `400 Bad Request` — request validation fails
- `404 Not Found` — actor does not exist

## Movies

### `GET /api/movies`

Lists movies ordered by title.

Optional query parameters:

| Parameter | Type | Description |
|---|---|---|
| `genre` | string | Case-insensitive substring match against genre names |
| `year` | integer | Exact release-year match |
| `actor` | string | Substring match against associated actor names |
| `withActors` | boolean | Include actors and their movie roles; default is `false` |

Example:

```text
GET /api/movies?year=2008&genre=Crime&withActors=true
```

**Response:** `200 OK`

```json
[
  {
    "id": 3,
    "title": "The Dark Knight",
    "year": 2008,
    "duration": 152,
    "directorId": 3,
    "directorName": "Christopher Nolan",
    "countryId": 1,
    "countryName": "United States",
    "genres": ["Action", "Crime", "Drama"],
    "actors": [
      {
        "id": 5,
        "name": "Christian Bale",
        "birthYear": 1974,
        "role": "Bruce Wayne / Batman"
      }
    ]
  }
]
```

### `GET /api/movies/{id}`

Returns a movie by ID.

Optional query parameter: `withActors=true`.

- `200 OK` — movie found
- `404 Not Found` — movie does not exist

### `GET /api/movies/{id}/details`

Returns the movie's complete detail projection, including:

- basic movie fields
- director and country names
- genres
- actors and roles
- `MovieDetails` synopsis, language, and budget
- all reviews

- `200 OK` — details found
- `404 Not Found` — movie does not exist

### `POST /api/movies`

Creates a movie. `directorId`, `countryId`, and all `genreIds` must identify existing records.

```json
{
  "title": "Inception",
  "year": 2010,
  "duration": 148,
  "directorId": 3,
  "countryId": 1,
  "genreIds": [1, 3],
  "actorIds": [5, 6]
}
```

- `201 Created` — returns `MovieResponse`
- `400 Bad Request` — director, country, genre, or actor ID is invalid
- `400 Bad Request` — request validation fails

### `PUT /api/movies/{id}`

Replaces the mutable movie fields and updates genres and actor associations.

```json
{
  "title": "The Dark Knight",
  "year": 2008,
  "duration": 152,
  "directorId": 3,
  "countryId": 1,
  "genreIds": [1, 2, 3],
  "actorIds": [5, 6, 4]
}
```

- `204 No Content` — movie updated
- `400 Bad Request` — referenced ID or input is invalid
- `404 Not Found` — movie does not exist

### `DELETE /api/movies/{id}`

Deletes a movie and related associations according to EF Core relationship configuration.

- `204 No Content` — movie deleted
- `404 Not Found` — movie does not exist

### `POST /api/movies/{movieId}/actors`

Adds an actor and role using an actor ID in the body.

```json
{
  "actorId": 4,
  "role": "Main antagonist"
}
```

- `201 Created` — returns the created movie-actor association
- `404 Not Found` — movie or actor does not exist
- `409 Conflict` — the movie/actor pair already exists

### `POST /api/movies/{movieId}/actors/{actorId}`

Alternative association endpoint that receives the actor ID in the route.

```json
{
  "role": "Supporting character"
}
```

The same `201`, `404`, and `409` responses apply.

## Reviews

### `GET /api/movies/{movieId}/reviews`

Returns reviews for a movie ordered by review ID.

- `200 OK` — returns an array of reviews
- `404 Not Found` — movie does not exist

```json
[
  {
    "id": 1,
    "reviewerName": "Alex",
    "comment": "A moving drama with memorable performances.",
    "rating": 5
  }
]
```

### `POST /api/movies/{movieId}/reviews`

Creates a review for an existing movie.

```json
{
  "reviewerName": "Alex",
  "comment": "Excellent pacing and performances.",
  "rating": 5
}
```

- `201 Created` — returns the created review
- `400 Bad Request` — request validation fails
- `404 Not Found` — movie does not exist

### `DELETE /api/reviews/{id}`

Deletes a review.

- `204 No Content` — review deleted
- `404 Not Found` — review does not exist

## Reports

All report endpoints are rooted at `/api/reports` and use EF Core projections or `AsNoTracking()` for read-only queries.

### `GET /api/reports/movies/top5pergenre`

Returns each genre and up to five highest-rated movies containing a review. Ordering is average rating descending, then title ascending.

### `GET /api/reports/movies/average-ratings`

Returns every genre's average review rating and review count.

- `AverageRating` is `null` when a genre has no reviews.
- Genres are ordered by name.

### `GET /api/reports/actors/most-active`

Returns all actors tied for the highest number of movie associations.

### `GET /api/reports/movies/longest-per-country`

Returns the longest movie, or all longest-duration ties, for every country represented in the movie table.

### `GET /api/reports/movies/with-most-reviews`

Returns every movie tied for the maximum review count. An empty database returns an empty array.

### `GET /api/reports/genres/popular`

Returns genres ordered by descending movie count, then genre name.

## Request DTOs and validation

| Request | Field | Constraints |
|---|---|---|
| `CreateActorRequest` / `UpdateActorRequest` | `Name` | Required, 1–150 characters |
|  | `BirthYear` | Range 1888–2100 |
| `CreateMovieRequest` / `UpdateMovieRequest` | `Title` | Required, 1–200 characters |
|  | `Year` | Range 1888–2100 |
|  | `Duration` | Range 1–1000 minutes |
|  | `DirectorId` | Positive integer and must exist |
|  | `CountryId` | Positive integer and must exist |
|  | `GenreIds` | Required collection with at least one ID |
|  | `ActorIds` | Optional collection; every ID must exist |
| `AddMovieActorRequest` / `AddMovieActorRoleRequest` | `Role` | Required, 1–100 characters |
| `CreateReviewRequest` | `ReviewerName` | Required, 1–100 characters |
|  | `Comment` | Required, 1–2000 characters |
|  | `Rating` | Integer range 1–5 |

`[ApiController]` enables automatic model-state validation. Invalid payloads return `400 Bad Request` before action logic is invoked.

## Entity model

`MovieDbContext` exposes these EF Core sets:

- `Movies`
- `MovieDetails`
- `Reviews`
- `Actors`
- `Genres`
- `Directors`
- `Countries`
- `MovieActors`

Important relationships:

- A movie has one director and one country.
- A movie has one optional details record.
- A movie has many reviews.
- A movie has many genres.
- `MovieActor` is an explicit join entity.
- `MovieActor` uses the composite key `(MovieId, ActorId)` and stores a `Role`.
- Fluent configurations are discovered with `ApplyConfigurationsFromAssembly`.

## Architecture

The project separates responsibilities through:

- **Controllers** — HTTP endpoints and status-code responses
- **DTOs** — request/response records and validation
- **Mappings** — entity/DTO conversion extensions
- **Repositories** — entity-specific database operations
- **IUnitOfWork** — grouped repository access and `SaveChangesAsync` coordination
- **Data/Configurations** — EF Core Fluent API mappings
- **Infrastructure/GlobalExceptionHandler** — centralized exception-to-Problem Details handling
- **Extensions** — SQL Server registration, migrations, and seed data

## Database startup and seed data

`SeedDataAsync()` is called immediately after the application is built to apply pending migrations. EF Core's configured `UseAsyncSeeding` callback then:

1. Leaves existing data unchanged if the `Movies` table contains records.
2. Seeds genres, directors, countries, actors, movies, movie details, roles, and reviews when the database is empty.

The matching `UseSeeding` callback provides the same behavior for EF Core tooling that invokes the synchronous seeding hook.

## Global exception handling

`GlobalExceptionHandler` implements `IExceptionHandler` and maps exceptions to RFC 7807 Problem Details responses:

| Exception | Status |
|---|---|
| `ArgumentException` or `BadHttpRequestException` | `400 Bad Request` |
| `KeyNotFoundException` | `404 Not Found` |
| `UnauthorizedAccessException` | `401 Unauthorized` |
| Any other exception | `500 Internal Server Error` |

Unexpected `500` responses hide the underlying exception message. The exception is logged by the handler.

## Read-query techniques

The implementation uses:

- `AsNoTracking()` for read-only list/detail queries
- `Include` and `ThenInclude` for eager loading navigation properties
- `AsSplitQuery()` for movie detail graphs
- Projection-based detail queries
- Server-side filtering for genre, year, and actor names
- `Any`, `Contains`, `GroupBy`, `Join`, `SelectMany`, `Max`, `Average`, and `Count` for reports
- Duplicate-safe relationship updates using distinct IDs

## HTTP status behavior

| Status | Meaning |
|---|---|
| `200 OK` | Resource or collection returned |
| `201 Created` | Resource created |
| `204 No Content` | Update or delete succeeded |
| `400 Bad Request` | DTO validation or invalid relationship ID |
| `401 Unauthorized` | Unauthorized exception |
| `404 Not Found` | Referenced resource does not exist |
| `409 Conflict` | Actor is already associated with the movie |
| `500 Internal Server Error` | Unexpected unhandled exception |

## API documentation

In Development only:

- OpenAPI document: `/openapi/v1.json`
- Scalar API reference: mapped by `MapScalarApiReference()`

The exact Scalar route depends on its configured defaults, usually the development application root.

## Project structure

```text
MovieApi/
├── Controllers/       API endpoints
├── DTOs/
│   ├── Actors/        Actor request and response records
│   ├── Mappings/      Entity/DTO mapping extensions
│   ├── MovieActors/   Movie-actor association DTOs
│   ├── MovieDetails/  Movie-details responses
│   ├── Movies/        Movie request and response records
│   ├── Reports/       Report response records
│   └── Reviews/       Review request and response records
├── Data/
│   ├── Configurations/ EF Core Fluent API configurations
│   ├── IUnitOfWork.cs
│   ├── MovieDbContext.cs
│   └── UnitOfWork.cs
├── Extensions/        Persistence, migration, and seed helpers
├── Infrastructure/    Global exception handling
├── Migrations/        EF Core migration history
├── Models/            Database entities
├── Repositories/
│   ├── Interfaces/    Repository contracts
│   └── ...            Entity repositories and report repository
├── Properties/        Launch settings
├── appsettings.json   SQL Server and logging configuration
└── Program.cs         Dependency injection and middleware setup
```

## Notes

- Authentication/authorization middleware is registered, but there are currently no protected endpoints or authorization policies.
- Controllers use a dependency-injected `IUnitOfWork` with entity-specific repositories.
- HTTPS redirection is enabled in the application pipeline.
- The global exception handler is registered before the application pipeline runs.
- The startup seed helper applies migrations and initializes data when appropriate.
