using MovieApi.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MovieApi.Repositories.Interfaces;

public interface IActorRepository
{
    Task<List<Actor>> GetAllAsync(CancellationToken ct = default);
    Task<Actor?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Actor?> FindByIdAsync(int id, CancellationToken ct = default);
    Task<List<Actor>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken ct = default);
    void Add(Actor actor);
}