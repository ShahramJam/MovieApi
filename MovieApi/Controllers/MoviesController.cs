using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MovieApi.Data;
using MovieApi.DTOs.Actors;
using MovieApi.DTOs.Errors;
using MovieApi.DTOs.Mappings;
using MovieApi.DTOs.MovieActors;
using MovieApi.DTOs.Movies;
using MovieApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MovieApi.Controllers;

[Route("api/movies")]
[ApiController]
public class MoviesController(IUnitOfWork unitOfWork) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    [HttpGet]
    public async Task<Ok<IReadOnlyList<MovieResponse>>> GetMovies(
        [FromQuery] string? genre,
        [FromQuery] int? year,
        [FromQuery] string? actor,
        [FromQuery] bool withActors = false,
        CancellationToken ct = default)
    {
        var movies = await _unitOfWork.Movies.GetMoviesAsync(genre, year, actor, withActors, ct);
        return TypedResults.Ok<IReadOnlyList<MovieResponse>>(
            [.. movies.Select(movie => movie.ToResponse(withActors))]);
    }

    [HttpGet("{id:int}", Name = nameof(GetMovie))]
    public async Task<Results<Ok<MovieResponse>, NotFound>> GetMovie(
        int id,
        [FromQuery] bool withActors = false,
        CancellationToken ct = default)
    {
        var movie = await _unitOfWork.Movies.GetByIdAsync(id, withActors, ct);
        if (movie is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(movie.ToResponse(withActors));
    }

    [HttpGet("{id:int}/details", Name = nameof(GetMovieDetails))]
    public async Task<Results<Ok<MovieDetailResponse>, NotFound>> GetMovieDetails(int id, CancellationToken ct = default)
    {
        var movie = await _unitOfWork.Movies.GetDetailsByIdAsync(id, ct);

        if (movie is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(movie);
    }

    [HttpPost]
    public async Task<Results<CreatedAtRoute<MovieResponse>, BadRequest<ApiErrorResponse>>> CreateMovie(
        CreateMovieRequest request,
        CancellationToken ct = default)
    {
        var director = await FindDirectorAsync(request.DirectorId, ct);
        if (director is null)
        {
            return TypedResults.BadRequest(new ApiErrorResponse("The director ID does not exist."));
        }

        var country = await FindCountryAsync(request.CountryId, ct);
        if (country is null)
        {
            return TypedResults.BadRequest(new ApiErrorResponse("The country ID does not exist."));
        }

        var genres = await FindGenresAsync(request.GenreIds, ct);
        if (genres is null)
        {
            return TypedResults.BadRequest(new ApiErrorResponse("One or more genre IDs do not exist."));
        }

        var actors = await FindActorsAsync(request.ActorIds ?? new List<int>(), ct);
        if (actors is null)
        {
            return TypedResults.BadRequest(new ApiErrorResponse("One or more actor IDs do not exist."));
        }

        var movie = request.ToEntity(director, country, genres, actors);

        _unitOfWork.Movies.Add(movie);
        await _unitOfWork.SaveChangesAsync(ct);

        return TypedResults.CreatedAtRoute(
            movie.ToResponse(includeActors: true),
            nameof(GetMovie),
            new { id = movie.Id });
    }

    [HttpPut("{id:int}")]
    public async Task<Results<NoContent, NotFound, BadRequest<ApiErrorResponse>>> UpdateMovie(
        int id,
        UpdateMovieRequest request,
        CancellationToken ct = default)
    {
        var movie = await _unitOfWork.Movies.GetForUpdateAsync(id, ct);

        if (movie is null)
        {
            return TypedResults.NotFound();
        }

        var director = await FindDirectorAsync(request.DirectorId, ct);
        if (director is null)
        {
            return TypedResults.BadRequest(new ApiErrorResponse("The director ID does not exist."));
        }

        var country = await FindCountryAsync(request.CountryId, ct);
        if (country is null)
        {
            return TypedResults.BadRequest(new ApiErrorResponse("The country ID does not exist."));
        }

        var genres = await FindGenresAsync(request.GenreIds, ct);
        if (genres is null)
        {
            return TypedResults.BadRequest(new ApiErrorResponse("One or more genre IDs do not exist."));
        }

        var actors = await FindActorsAsync(request.ActorIds ?? new List<int>(), ct);
        if (actors is null)
        {
            return TypedResults.BadRequest(new ApiErrorResponse("One or more actor IDs do not exist."));
        }

        request.UpdateEntity(movie, director, country, genres, actors);

        await _unitOfWork.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<Results<NoContent, NotFound>> DeleteMovie(int id, CancellationToken ct = default)
    {
        var movie = await _unitOfWork.Movies.FindByIdAsync(id, ct);
        if (movie is null)
        {
            return TypedResults.NotFound();
        }

        _unitOfWork.Movies.Remove(movie);
        await _unitOfWork.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }

    [HttpPost("{movieId:int}/actors")]
    public async Task<Results<CreatedAtRoute<ActorResponse>, NotFound, Conflict<ApiErrorResponse>>> AddActorToMovie(
        int movieId,
        AddMovieActorRequest request,
        CancellationToken ct = default)
    {
        return await AddActorToMovieAsync(
            movieId,
            request.ActorId,
            actor => request.ToEntity(movieId, actor),
            ct);
    }

    [HttpPost("{movieId:int}/actors/{actorId:int}")]
    public async Task<Results<CreatedAtRoute<ActorResponse>, NotFound, Conflict<ApiErrorResponse>>> AddActorToMovie(
        int movieId,
        int actorId,
        AddMovieActorRoleRequest request,
        CancellationToken ct = default)
    {
        return await AddActorToMovieAsync(
            movieId,
            actorId,
            actor => request.ToEntity(movieId, actor),
            ct);
    }

    private async Task<Results<CreatedAtRoute<ActorResponse>, NotFound, Conflict<ApiErrorResponse>>> AddActorToMovieAsync(
        int movieId,
        int actorId,
        Func<Actor, MovieActor> createMovieActor,
        CancellationToken ct)
    {
        var movieExists = await _unitOfWork.Movies.ExistsAsync(movieId, ct);
        var actor = await _unitOfWork.Actors.FindByIdAsync(actorId, ct);
        if (!movieExists || actor is null)
        {
            return TypedResults.NotFound();
        }

        var associationExists = await _unitOfWork.MovieActors.ExistsAsync(movieId, actorId, ct);
        if (associationExists)
        {
            return TypedResults.Conflict(new ApiErrorResponse("This actor is already associated with the movie."));
        }

        var movieActor = createMovieActor(actor);
        _unitOfWork.MovieActors.Add(movieActor);
        await _unitOfWork.SaveChangesAsync(ct);

        return TypedResults.CreatedAtRoute(
            movieActor.ToResponse(),
            nameof(GetMovieDetails),
            new { id = movieId });
    }

    private async Task<List<Genre>?> FindGenresAsync(IEnumerable<int> ids, CancellationToken ct)
    {
        var uniqueIds = ids.Distinct().ToArray();
        var genres = await _unitOfWork.Genres.GetByIdsAsync(uniqueIds, ct);

        return genres.Count == uniqueIds.Length ? genres : null;
    }

    private Task<Director?> FindDirectorAsync(int id, CancellationToken ct)
    {
        return _unitOfWork.Directors.FindByIdAsync(id, ct);
    }

    private Task<Country?> FindCountryAsync(int id, CancellationToken ct)
    {
        return _unitOfWork.Countries.FindByIdAsync(id, ct);
    }

    private async Task<List<Actor>?> FindActorsAsync(IEnumerable<int> ids, CancellationToken ct)
    {
        var uniqueIds = ids.Distinct().ToArray();
        var actors = await _unitOfWork.Actors.GetByIdsAsync(uniqueIds, ct);

        return actors.Count == uniqueIds.Length ? actors : null;
    }

}