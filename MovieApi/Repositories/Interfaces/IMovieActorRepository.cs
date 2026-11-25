using MovieApi.Models;
using System.Threading;
using System.Threading.Tasks;

namespace MovieApi.Repositories.Interfaces;

public interface IMovieActorRepository
{
    Task<bool> ExistsAsync(int movieId, int actorId, CancellationToken ct = default);
    void Add(MovieActor movieActor);
}