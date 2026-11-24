# MovieApi

ASP.NET Core Web API for managing movies, actors, reviews, and aggregate reports. The project uses Entity Framework Core with SQL Server and exposes an OpenAPI document plus a Scalar API reference in the Development environment.

## Technologies

- .NET 10 / ASP.NET Core
- Entity Framework Core 10
- SQL Server
- OpenAPI (`Microsoft.AspNetCore.OpenApi`)
- Scalar API reference
- ASP.NET Core controller-based APIs
- Fluent EF Core entity configurations
- DTO mapping extension methods
- Automatic migrations and seed data startup

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

```bash
cd MovieApi
dotnet restore
dotnet run
```

On startup, the application:

1. Creates the database context.
2. Applies pending EF Core migrations with `Database.Migrate()`.
3. Seeds initial data only when no movies exist.
4. Starts the HTTPS redirect middleware.
5. Maps controllers and, in Development, `/openapi/v1.json` and the Scalar API reference.

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

- `201 Created` — returns the created `ActorDto`
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

The response shape extends `MovieDto` with `movieDetails`, `reviews`, and `actors`.

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

- `201 Created` — returns `MovieDto`
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

Representative fields:

```text
GenreId, Genre, Movies[{ MovieId, Title, Year, AverageRating, ReviewCount }]
```

### `GET /api/reports/movies/average-ratings`

Returns every genre's average review rating and review count.

- `AverageRating` is `null` when a genre has no reviews.
- Genres are ordered by name.

```text
GenreId, Genre, AverageRating, ReviewCount
```

### `GET /api/reports/actors/most-active`

Returns all actors tied for the highest number of movie associations.

```text
ActorId, Name, MovieCount
```

### `GET /api/reports/movies/longest-per-country`

Returns the longest movie, or all longest-duration ties, for every country represented in the movie table.

```text
CountryId, Country, MovieId, Title, Year, Duration
```

### `GET /api/reports/movies/with-most-reviews`

Returns every movie tied for the maximum review count.

```text
MovieId, Title, Year, ReviewCount
```

An empty database returns an empty array.

### `GET /api/reports/genres/popular`

Returns genres ordered by descending movie count, then genre name.

```text
GenreId, Genre, MovieCount
```

## Request DTOs and validation

### `ActorCreateDto` / `ActorUpdateDto`

| Field | Constraints |
|---|---|
| `Name` | Required, 1–150 characters |
| `BirthYear` | Range 1888–2100 |

### `ActorDto`

Contains `Id`, `Name`, `BirthYear`, and optional `Role`. The role is included when an actor is projected as part of a movie.

### `MovieCreateDto` / `MovieUpdateDto`

| Field | Constraints |
|---|---|
| `Title` | Required, 1–200 characters |
| `Year` | Range 1888–2100 |
| `Duration` | Range 1–1000 minutes |
| `DirectorId` | Positive integer and must exist |
| `CountryId` | Positive integer and must exist |
| `GenreIds` | Required collection with at least one ID |
| `ActorIds` | Optional collection; every ID must exist |

`[ApiController]` enables automatic model-state validation. Invalid payloads return `400 Bad Request` before action logic is invoked.

### `MovieDto` / `MovieDetailDto`

`MovieDto` exposes the movie, director and country IDs/names, genre names, and optionally actors.

`MovieDetailDto` additionally exposes:

- `MovieDetails` — synopsis, language, budget, and details ID
- `Reviews` — reviewer name, comment, and rating
- `Actors` — actor data and roles

### `MovieActorCreateDto`

| Field | Constraints |
|---|---|
| `ActorId` | Positive integer |
| `Role` | Required, 1–100 characters |

### `MovieActorRoleDto`

| Field | Constraints |
|---|---|
| `Role` | Required, 1–100 characters |

### `ReviewDto`

| Field | Constraints |
|---|---|
| `Id` | Set by the database for create/update responses |
| `ReviewerName` | Required, 1–100 characters |
| `Comment` | Required, 1–2000 characters |
| `Rating` | Integer range 1–5 |

## Entity model

`MovieDbContext` exposes the following EF Core sets:

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
- Actor/movie associations are unique by the composite key.
- EF Fluent configurations are discovered with `ApplyConfigurationsFromAssembly`.

## Mappings

DTO and entity conversions are implemented through extension methods in `DTOs/Mappings`:

- `ActorMappings`
- `MovieActorMappings`
- `MovieDetailsMappings`
- `MovieMappings`
- `ReviewMappings`

This keeps controller payload creation and entity conversion separate from EF entities and database logic.

## Database startup and seed data

`SeedData()` is called immediately after the application is built. It:

1. Applies pending migrations.
2. If the `Movies` table already contains data, leaves existing data unchanged.
3. If empty, seeds genres, directors, countries, actors, movies, movie details, roles, and reviews.

Seeded movie titles include:

- The Shawshank Redemption
- The Matrix
- The Dark Knight

Seed helpers reuse named directors and countries or add them if absent.

## Read-query techniques

The implementation uses several EF Core techniques:

- `AsNoTracking()` for read-only list/detail queries.
- `Include` and `ThenInclude` for eager loading navigation properties.
- `AsSplitQuery()` for movie detail graphs to avoid join fan-out.
- projection-based detail queries through `SelectDetailDto()`.
- server-side filtering for genre, year, and actor names.
- `Any`, `Contains`, `GroupBy`, `Join`, `SelectMany`, `Max`, `Average`, and `Count` for reports.
- duplicate-safe relationship updates using distinct IDs.

## HTTP status behavior

| Status | Meaning |
|---|---|
| `200 OK` | Resource or collection returned |
| `201 Created` | Resource created; usually includes a `Location` value |
| `204 No Content` | Update or delete succeeded |
| `400 Bad Request` | DTO validation or invalid existing relationship ID |
| `404 Not Found` | Referenced resource does not exist |
| `409 Conflict` | Actor is already associated with the movie |

## API documentation

In Development only:

- OpenAPI document: `/openapi/v1.json`
- Scalar API reference: mapped by `MapScalarApiReference()`

The exact Scalar route depends on its configured defaults, usually the development application root.

## Project structure

```text
MovieApi/
├── Controllers/       API endpoints
├── DTOs/              Request and response models
│   ├── Mappings/      Entity/DTO mapping extensions
│   └── Reports/       Report response models
├── Data/              DbContext and Fluent EF configurations
├── Extensions/        Migration and seed startup helpers
├── Migrations/        EF Core migration history
├── Models/            Database entities
├── Properties/        Launch settings
├── appsettings.json   SQL Server and logging configuration
└── Program.cs         Dependency injection and middleware setup
```

## Notes

- Authentication/authorization middleware is registered, but there are currently no protected endpoints or authorization policies.
- All controllers use dependency-injected `MovieDbContext` instances.
- HTTPS redirection is enabled in the application pipeline.
- The project does not currently define a separate repository/service layer; controller actions query the EF Core context directly.
