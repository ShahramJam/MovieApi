using MovieApi.Models;
using System.Threading;
using System.Threading.Tasks;

namespace MovieApi.Repositories.Interfaces;

public interface IDirectorRepository
{
    Task<Director?> FindByIdAsync(int id, CancellationToken ct = default);
}