using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApi.Data;
using MovieApi.DTOs;
using MovieApi.DTOs.Mappings;
using MovieApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApi.Controllers;

[Route("api/movies")]
[ApiController]
public class MoviesController : ControllerBase
{
    private readonly MovieDbContext _context;

    public MoviesController(MovieDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovieDto>>> GetMovies(
        [FromQuery] string? genre,
        [FromQuery] int? year,
        [FromQuery] string? actor,
        [FromQuery] bool withActors = false)
    {
        var query = _context.Movies
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(genre))
        {
            var genreFilter = genre.Trim();
            query = query.Where(movie => movie.Genres.Any(item => item.Name.Contains(genreFilter)));
        }

        if (year.HasValue)
        {
            query = query.Where(movie => movie.Year == year.Value);
        }

        if (!string.IsNullOrWhiteSpace(actor))
        {
            var actorFilter = actor.Trim();
            query = query.Where(movie => movie.MovieActors.Any(
                movieActor => movieActor.Actor.Name.Contains(actorFilter)));
        }

        query = query
            .Include(movie => movie.Genres)
            .OrderBy(movie => movie.Title);

        if (withActors)
        {
            query = query
                .Include(movie => movie.MovieActors)
                .ThenInclude(movieActor => movieActor.Actor);
        }

        var movies = await query.AsSplitQuery().ToListAsync();
        return movies.Select(movie => movie.ToDto(withActors)).ToList();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MovieDto>> GetMovie(int id, [FromQuery] bool withActors = false)
    {
        var query = _context.Movies
            .AsNoTracking()
            .Include(movie => movie.Genres)
            .AsQueryable();

        if (withActors)
        {
            query = query
                .Include(movie => movie.MovieActors)
                .ThenInclude(movieActor => movieActor.Actor);
        }

        var movie = await query
            .AsSplitQuery()
            .FirstOrDefaultAsync(item => item.Id == id);
        if (movie is null)
        {
            return NotFound();
        }

        return movie.ToDto(withActors);
    }

    [HttpGet("{id:int}/details")]
    public async Task<ActionResult<MovieDetailDto>> GetMovieDetails(int id)
    {
        var movie = await _context.Movies
            .AsNoTracking()
            .Where(item => item.Id == id)
            .AsSplitQuery()
            .SelectDetailDto()
            .FirstOrDefaultAsync();

        if (movie is null)
        {
            return NotFound();
        }

        return movie;
    }

    [HttpPost]
    public async Task<ActionResult<MovieDto>> CreateMovie(MovieCreateDto movieDto)
    {
        var genres = await FindGenresAsync(movieDto.GenreIds);
        if (genres is null)
        {
            return BadRequest(new { message = "One or more genre IDs do not exist." });
        }

        var actors = await FindActorsAsync(movieDto.ActorIds);
        if (actors is null)
        {
            return BadRequest(new { message = "One or more actor IDs do not exist." });
        }

        var movie = movieDto.ToEntity(genres, actors);

        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetMovie),
            new { id = movie.Id },
            movie.ToDto(includeActors: true));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateMovie(int id, MovieUpdateDto movieDto)
    {
        var movie = await _context.Movies
            .Include(item => item.Genres)
            .Include(item => item.MovieActors)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (movie is null)
        {
            return NotFound();
        }

        var genres = await FindGenresAsync(movieDto.GenreIds);
        if (genres is null)
        {
            return BadRequest(new { message = "One or more genre IDs do not exist." });
        }

        var actors = await FindActorsAsync(movieDto.ActorIds);
        if (actors is null)
        {
            return BadRequest(new { message = "One or more actor IDs do not exist." });
        }

        movieDto.UpdateEntity(movie, genres, actors);

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteMovie(int id)
    {
        var movie = await _context.Movies.FindAsync(id);
        if (movie is null)
        {
            return NotFound();
        }

        _context.Movies.Remove(movie);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{movieId:int}/actors/{actorId:int}")]
    public async Task<IActionResult> AddActorToMovie(
        int movieId,
        int actorId,
        MovieActorCreateDto movieActorDto)
    {
        var movieExists = await _context.Movies.AnyAsync(movie => movie.Id == movieId);
        var actor = await _context.Actors.FindAsync(actorId);
        if (!movieExists || actor is null)
        {
            return NotFound();
        }

        var associationExists = await _context.MovieActors.AnyAsync(
            movieActor => movieActor.MovieId == movieId && movieActor.ActorId == actorId);
        if (associationExists)
        {
            return Conflict(new { message = "This actor is already associated with the movie." });
        }

        var movieActor = movieActorDto.ToEntity(movieId, actor);
        _context.MovieActors.Add(movieActor);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetMovieDetails),
            new { id = movieId },
            movieActor.ToDto());
    }

    private async Task<List<Genre>?> FindGenresAsync(IEnumerable<int> ids)
    {
        var uniqueIds = ids.Distinct().ToArray();
        var genres = await _context.Genres
            .Where(genre => uniqueIds.Contains(genre.Id))
            .ToListAsync();

        return genres.Count == uniqueIds.Length ? genres : null;
    }

    private async Task<List<Actor>?> FindActorsAsync(IEnumerable<int> ids)
    {
        var uniqueIds = ids.Distinct().ToArray();
        var actors = await _context.Actors
            .Where(actor => uniqueIds.Contains(actor.Id))
            .ToListAsync();

        return actors.Count == uniqueIds.Length ? actors : null;
    }

}