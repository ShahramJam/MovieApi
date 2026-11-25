using Microsoft.EntityFrameworkCore;
using MovieApi.Data;
using MovieApi.Models;
using MovieApi.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MovieApi.Repositories;

public sealed class ReviewRepository(MovieDbContext context) : IReviewRepository
{
    private readonly MovieDbContext _context = context;

    public Task<List<Review>> GetByMovieIdAsync(int movieId, CancellationToken ct = default)
    {
        return _context.Reviews
            .AsNoTracking()
            .Where(review => review.MovieId == movieId)
            .OrderBy(review => review.Id)
            .ToListAsync(ct);
    }

    public async Task<Review?> FindByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Reviews.FindAsync(new object?[] { id }, ct);
    }

    public void Add(Review review)
    {
        _context.Reviews.Add(review);
    }

    public void Remove(Review review)
    {
        _context.Reviews.Remove(review);
    }
}