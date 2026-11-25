using Microsoft.EntityFrameworkCore;
using MovieApi.Data;
using MovieApi.Models;
using MovieApi.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MovieApi.Repositories;

public sealed class GenreRepository(MovieDbContext context) : IGenreRepository
{
    private readonly MovieDbContext _context = context;

    public async Task<List<Genre>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken ct = default)
    {
        var uniqueIds = ids.Distinct().ToArray();
        return await _context.Genres
            .Where(genre => uniqueIds.Contains(genre.Id))
            .ToListAsync(ct);
    }
}