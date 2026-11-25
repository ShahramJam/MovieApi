using Microsoft.EntityFrameworkCore;
using MovieApi.Data;
using MovieApi.Models;
using MovieApi.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MovieApi.Repositories;

public sealed class ActorRepository(MovieDbContext context) : IActorRepository
{
    private readonly MovieDbContext _context = context;

    public Task<List<Actor>> GetAllAsync(CancellationToken ct = default)
    {
        return _context.Actors
            .AsNoTracking()
            .OrderBy(actor => actor.Name)
            .ToListAsync(ct);
    }

    public Task<Actor?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return _context.Actors
            .AsNoTracking()
            .FirstOrDefaultAsync(actor => actor.Id == id, ct);
    }

    public async Task<Actor?> FindByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Actors.FindAsync(new object?[] { id }, ct);
    }

    public async Task<List<Actor>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken ct = default)
    {
        var uniqueIds = ids.Distinct().ToArray();
        return await _context.Actors
            .Where(actor => uniqueIds.Contains(actor.Id))
            .ToListAsync(ct);
    }

    public void Add(Actor actor)
    {
        _context.Actors.Add(actor);
    }
}