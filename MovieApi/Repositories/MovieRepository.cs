using Microsoft.EntityFrameworkCore;
using MovieApi.Data;
using MovieApi.DTOs.Mappings;
using MovieApi.DTOs.Movies;
using MovieApi.Models;
using MovieApi.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MovieApi.Repositories;

public sealed class MovieRepository(MovieDbContext context) : IMovieRepository
{
    private readonly MovieDbContext _context = context;

    public async Task<List<Movie>> GetMoviesAsync(
        string? genre,
        int? year,
        string? actor,
        bool withActors,
        CancellationToken ct = default)
    {
        var query = _context.Movies
            .AsNoTracking()
            .Include(movie => movie.Director)
            .Include(movie => movie.Country)
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

        return await query.AsSplitQuery().ToListAsync(ct);
    }

    public async Task<Movie?> GetByIdAsync(int id, bool withActors, CancellationToken ct = default)
    {
        var query = _context.Movies
            .AsNoTracking()
            .Include(movie => movie.Director)
            .Include(movie => movie.Country)
            .Include(movie => movie.Genres)
            .AsQueryable();

        if (withActors)
        {
            query = query
                .Include(movie => movie.MovieActors)
                .ThenInclude(movieActor => movieActor.Actor);
        }

        return await query
            .AsSplitQuery()
            .FirstOrDefaultAsync(movie => movie.Id == id, ct);
    }

    public async Task<MovieDetailResponse?> GetDetailsByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Movies
            .AsNoTracking()
            .Where(movie => movie.Id == id)
            .AsSplitQuery()
            .SelectDetailResponse()
            .FirstOrDefaultAsync(ct);
    }

    public async Task<Movie?> GetForUpdateAsync(int id, CancellationToken ct = default)
    {
        return await _context.Movies
            .Include(movie => movie.Genres)
            .Include(movie => movie.MovieActors)
            .FirstOrDefaultAsync(movie => movie.Id == id, ct);
    }

    public async Task<Movie?> FindByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Movies.FindAsync(new object?[] { id }, ct);
    }

    public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
    {
        return _context.Movies.AnyAsync(movie => movie.Id == id, ct);
    }

    public void Add(Movie movie)
    {
        _context.Movies.Add(movie);
    }

    public void Remove(Movie movie)
    {
        _context.Movies.Remove(movie);
    }
}