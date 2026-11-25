using Microsoft.EntityFrameworkCore;
using MovieApi.Data;
using MovieApi.Models;
using MovieApi.Repositories.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace MovieApi.Repositories;

public sealed class MovieActorRepository(MovieDbContext context) : IMovieActorRepository
{
    private readonly MovieDbContext _context = context;

    public Task<bool> ExistsAsync(int movieId, int actorId, CancellationToken ct = default)
    {
        return _context.MovieActors.AnyAsync(
            movieActor => movieActor.MovieId == movieId && movieActor.ActorId == actorId,
            ct);
    }

    public void Add(MovieActor movieActor)
    {
        _context.MovieActors.Add(movieActor);
    }
}