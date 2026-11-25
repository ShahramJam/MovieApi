using MovieApi.Data;
using MovieApi.Models;
using MovieApi.Repositories.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace MovieApi.Repositories;

public sealed class DirectorRepository(MovieDbContext context) : IDirectorRepository
{
    private readonly MovieDbContext _context = context;

    public async Task<Director?> FindByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Directors.FindAsync(new object?[] { id }, ct);
    }
}