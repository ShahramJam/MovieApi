using MovieApi.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MovieApi.Repositories.Interfaces;

public interface IReviewRepository
{
    Task<List<Review>> GetByMovieIdAsync(int movieId, CancellationToken ct = default);
    Task<Review?> FindByIdAsync(int id, CancellationToken ct = default);
    void Add(Review review);
    void Remove(Review review);
}