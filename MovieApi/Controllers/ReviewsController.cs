using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MovieApi.Data;
using MovieApi.DTOs.Mappings;
using MovieApi.DTOs.Reviews;
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

    [HttpGet("movies/{movieId:int}/reviews", Name = nameof(GetMovieReviews))]
    public async Task<Results<Ok<IReadOnlyList<ReviewResponse>>, NotFound>> GetMovieReviews(
        int movieId,
        CancellationToken ct = default)
    {
        var movieExists = await _unitOfWork.Movies.ExistsAsync(movieId, ct);
        if (!movieExists)
        {
            return TypedResults.NotFound();
        }

        var reviews = await _unitOfWork.Reviews.GetByMovieIdAsync(movieId, ct);

        return TypedResults.Ok<IReadOnlyList<ReviewResponse>>(
            [.. reviews.Select(review => review.ToResponse())]);
    }

    [HttpPost("movies/{movieId:int}/reviews")]
    public async Task<Results<CreatedAtRoute<ReviewResponse>, NotFound>> CreateMovieReview(
        int movieId,
        CreateReviewRequest request,
        CancellationToken ct = default)
    {
        var movieExists = await _unitOfWork.Movies.ExistsAsync(movieId, ct);
        if (!movieExists)
        {
            return TypedResults.NotFound();
        }

        var review = request.ToEntity(movieId);

        _unitOfWork.Reviews.Add(review);
        await _unitOfWork.SaveChangesAsync(ct);

        return TypedResults.CreatedAtRoute(
            review.ToResponse(),
            nameof(GetMovieReviews),
            new { movieId });
    }

    [HttpDelete("reviews/{id:int}")]
    public async Task<Results<NoContent, NotFound>> DeleteReview(int id, CancellationToken ct = default)
    {
        var review = await _unitOfWork.Reviews.FindByIdAsync(id, ct);
        if (review is null)
        {
            return TypedResults.NotFound();
        }

        _unitOfWork.Reviews.Remove(review);
        await _unitOfWork.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }
}