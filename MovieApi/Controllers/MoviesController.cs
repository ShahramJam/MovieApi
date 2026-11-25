using Microsoft.AspNetCore.Mvc;
using MovieApi.Data;
using MovieApi.DTOs.MovieActors;
using MovieApi.DTOs.Movies;
using MovieApi.DTOs.Mappings;
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
    public async Task<ActionResult<IEnumerable<MovieDto>>> GetMovies(
        [FromQuery] string? genre,
        [FromQuery] int? year,
        [FromQuery] string? actor,
        [FromQuery] bool withActors = false,
        CancellationToken ct = default)
    {
        var movies = await _unitOfWork.Movies.GetMoviesAsync(genre, year, actor, withActors, ct);
        return movies.Select(movie => movie.ToDto(withActors)).ToList();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MovieDto>> GetMovie(
        int id,
        [FromQuery] bool withActors = false,
        CancellationToken ct = default)
    {
        var movie = await _unitOfWork.Movies.GetByIdAsync(id, withActors, ct);
        if (movie is null)
        {
            return NotFound();
        }

        return movie.ToDto(withActors);
    }

    [HttpGet("{id:int}/details")]
    public async Task<ActionResult<MovieDetailDto>> GetMovieDetails(int id, CancellationToken ct = default)
    {
        var movie = await _unitOfWork.Movies.GetDetailsByIdAsync(id, ct);

        if (movie is null)
        {
            return NotFound();
        }

        return movie;
    }

    [HttpPost]
    public async Task<ActionResult<MovieDto>> CreateMovie(MovieCreateDto movieDto, CancellationToken ct = default)
    {
        var director = await FindDirectorAsync(movieDto.DirectorId, ct);
        if (director is null)
        {
            return BadRequest(new { message = "The director ID does not exist." });
        }

        var country = await FindCountryAsync(movieDto.CountryId, ct);
        if (country is null)
        {
            return BadRequest(new { message = "The country ID does not exist." });
        }

        var genres = await FindGenresAsync(movieDto.GenreIds, ct);
        if (genres is null)
        {
            return BadRequest(new { message = "One or more genre IDs do not exist." });
        }

        var actors = await FindActorsAsync(movieDto.ActorIds, ct);
        if (actors is null)
        {
            return BadRequest(new { message = "One or more actor IDs do not exist." });
        }

        var movie = movieDto.ToEntity(director, country, genres, actors);

        _unitOfWork.Movies.Add(movie);
        await _unitOfWork.SaveChangesAsync(ct);

        return CreatedAtAction(
            nameof(GetMovie),
            new { id = movie.Id },
            movie.ToDto(includeActors: true));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateMovie(int id, MovieUpdateDto movieDto, CancellationToken ct = default)
    {
        var movie = await _unitOfWork.Movies.GetForUpdateAsync(id, ct);

        if (movie is null)
        {
            return NotFound();
        }

        var director = await FindDirectorAsync(movieDto.DirectorId, ct);
        if (director is null)
        {
            return BadRequest(new { message = "The director ID does not exist." });
        }

        var country = await FindCountryAsync(movieDto.CountryId, ct);
        if (country is null)
        {
            return BadRequest(new { message = "The country ID does not exist." });
        }

        var genres = await FindGenresAsync(movieDto.GenreIds, ct);
        if (genres is null)
        {
            return BadRequest(new { message = "One or more genre IDs do not exist." });
        }

        var actors = await FindActorsAsync(movieDto.ActorIds, ct);
        if (actors is null)
        {
            return BadRequest(new { message = "One or more actor IDs do not exist." });
        }

        movieDto.UpdateEntity(movie, director, country, genres, actors);

        await _unitOfWork.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteMovie(int id, CancellationToken ct = default)
    {
        var movie = await _unitOfWork.Movies.FindByIdAsync(id, ct);
        if (movie is null)
        {
            return NotFound();
        }

        _unitOfWork.Movies.Remove(movie);
        await _unitOfWork.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpPost("{movieId:int}/actors")]
    public async Task<IActionResult> AddActorToMovie(
        int movieId,
        MovieActorCreateDto movieActorDto,
        CancellationToken ct = default)
    {
        return await AddActorToMovieAsync(
            movieId,
            movieActorDto.ActorId,
            actor => movieActorDto.ToEntity(movieId, actor),
            ct);
    }

    [HttpPost("{movieId:int}/actors/{actorId:int}")]
    public async Task<IActionResult> AddActorToMovie(
        int movieId,
        int actorId,
        MovieActorRoleDto movieActorDto,
        CancellationToken ct = default)
    {
        return await AddActorToMovieAsync(
            movieId,
            actorId,
            actor => movieActorDto.ToEntity(movieId, actor),
            ct);
    }

    private async Task<IActionResult> AddActorToMovieAsync(
        int movieId,
        int actorId,
        Func<Actor, MovieActor> createMovieActor,
        CancellationToken ct)
    {
        var movieExists = await _unitOfWork.Movies.ExistsAsync(movieId, ct);
        var actor = await _unitOfWork.Actors.FindByIdAsync(actorId, ct);
        if (!movieExists || actor is null)
        {
            return NotFound();
        }

        var associationExists = await _unitOfWork.MovieActors.ExistsAsync(movieId, actorId, ct);
        if (associationExists)
        {
            return Conflict(new { message = "This actor is already associated with the movie." });
        }

        var movieActor = createMovieActor(actor);
        _unitOfWork.MovieActors.Add(movieActor);
        await _unitOfWork.SaveChangesAsync(ct);

        return CreatedAtAction(
            nameof(GetMovieDetails),
            new { id = movieId },
            movieActor.ToDto());
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