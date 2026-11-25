using MovieApi.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MovieApi.Repositories.Interfaces;

public interface IGenreRepository
{
    Task<List<Genre>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken ct = default);
}