using MovieApi.Models;
using System.Threading;
using System.Threading.Tasks;

namespace MovieApi.Repositories.Interfaces;

public interface ICountryRepository
{
    Task<Country?> FindByIdAsync(int id, CancellationToken ct = default);
}