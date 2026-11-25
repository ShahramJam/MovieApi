using MovieApi.Data;
using MovieApi.Models;
using MovieApi.Repositories.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace MovieApi.Repositories;

public sealed class CountryRepository(MovieDbContext context) : ICountryRepository
{
    private readonly MovieDbContext _context = context;

    public async Task<Country?> FindByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Countries.FindAsync(new object?[] { id }, ct);
    }
}