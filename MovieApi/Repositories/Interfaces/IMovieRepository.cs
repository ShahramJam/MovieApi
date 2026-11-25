using MovieApi.DTOs.Movies;
using MovieApi.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MovieApi.Repositories.Interfaces;

public interface IMovieRepository
{
    Task<List<Movie>> GetMoviesAsync(string? genre, int? year, string? actor, bool withActors, CancellationToken ct = default);
    Task<Movie?> GetByIdAsync(int id, bool withActors, CancellationToken ct = default);
    Task<MovieDetailResponse?> GetDetailsByIdAsync(int id, CancellationToken ct = default);
    Task<Movie?> GetForUpdateAsync(int id, CancellationToken ct = default);
    Task<Movie?> FindByIdAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    void Add(Movie movie);
    void Remove(Movie movie);
}