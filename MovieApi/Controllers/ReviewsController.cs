using Microsoft.AspNetCore.Mvc;
using MovieApi.Data;
using MovieApi.DTOs;
using MovieApi.DTOs.Mappings;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MovieApi.Controllers;

[Route("api")]
[ApiController]
public class ReviewsController(IUnitOfWork unitOfWork) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    [HttpGet("movies/{movieId:int}/reviews")]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetMovieReviews(int movieId, CancellationToken ct = default)
    {
        var movieExists = await _unitOfWork.Movies.ExistsAsync(movieId, ct);
        if (!movieExists)
        {
            return NotFound();
        }

        var reviews = await _unitOfWork.Reviews.GetByMovieIdAsync(movieId, ct);

        return reviews.Select(review => review.ToDto()).ToList();
    }

    [HttpPost("movies/{movieId:int}/reviews")]
    public async Task<ActionResult<ReviewDto>> CreateMovieReview(
        int movieId,
        ReviewDto reviewDto,
        CancellationToken ct = default)
    {
        var movieExists = await _unitOfWork.Movies.ExistsAsync(movieId, ct);
        if (!movieExists)
        {
            return NotFound();
        }

        var review = reviewDto.ToEntity(movieId);

        _unitOfWork.Reviews.Add(review);
        await _unitOfWork.SaveChangesAsync(ct);

        return CreatedAtAction(
            nameof(GetMovieReviews),
            new { movieId },
            review.ToDto());
    }

    [HttpDelete("reviews/{id:int}")]
    public async Task<IActionResult> DeleteReview(int id, CancellationToken ct = default)
    {
        var review = await _unitOfWork.Reviews.FindByIdAsync(id, ct);
        if (review is null)
        {
            return NotFound();
        }

        _unitOfWork.Reviews.Remove(review);
        await _unitOfWork.SaveChangesAsync(ct);

        return NoContent();
    }
}