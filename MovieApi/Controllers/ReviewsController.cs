using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApi.Data;
using MovieApi.DTOs;
using MovieApi.DTOs.Mappings;

namespace MovieApi.Controllers;

[Route("api")]
[ApiController]
public class ReviewsController : ControllerBase
{
    private readonly MovieDbContext _context;

    public ReviewsController(MovieDbContext context)
    {
        _context = context;
    }

    [HttpGet("movies/{movieId:int}/reviews")]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetMovieReviews(int movieId)
    {
        var movieExists = await _context.Movies.AnyAsync(movie => movie.Id == movieId);
        if (!movieExists)
        {
            return NotFound();
        }

        var reviews = await _context.Reviews
            .AsNoTracking()
            .Where(review => review.MovieId == movieId)
            .OrderBy(review => review.Id)
            .ToListAsync();

        return reviews.Select(review => review.ToDto()).ToList();
    }

    [HttpPost("movies/{movieId:int}/reviews")]
    public async Task<ActionResult<ReviewDto>> CreateMovieReview(int movieId, ReviewDto reviewDto)
    {
        var movieExists = await _context.Movies.AnyAsync(movie => movie.Id == movieId);
        if (!movieExists)
        {
            return NotFound();
        }

        var review = reviewDto.ToEntity(movieId);

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetMovieReviews),
            new { movieId },
            review.ToDto());
    }

    [HttpDelete("reviews/{id:int}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review is null)
        {
            return NotFound();
        }

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}