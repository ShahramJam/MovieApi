# MovieApi

SQL Server-backed movie API using Entity Framework Core. `MovieActor` is an explicit join entity with a composite key and a `Role`; movies require a director and a country, and can be associated with multiple genres.

Movie create/update requests must include existing `directorId` and `countryId` values. Their names and IDs are returned by the movie endpoints.

Add an actor and role to a movie with `POST /api/movies/{movieId}/actors`:

```json
{
  "actorId": 4,
  "role": "Main antagonist"
}
```

Reports are available under `/api/reports`:

- `GET /movies/top5pergenre`
- `GET /movies/average-ratings`
- `GET /actors/most-active`
- `GET /movies/longest-per-country`
- `GET /movies/with-most-reviews`
- `GET /genres/popular`

The application applies pending migrations at startup before seeding data. Existing movies are assigned a known director/country when their title matches seed data; otherwise they receive an `Unknown` reference so the new required relationships preserve existing rows.
