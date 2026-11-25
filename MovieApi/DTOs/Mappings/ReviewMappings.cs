using MovieApi.DTOs.Reviews;
using MovieApi.Models;

namespace MovieApi.DTOs.Mappings;

public static class ReviewMappings
{
    public static ReviewResponse ToResponse(this Review review)
    {
        return new ReviewResponse(review.Id, review.ReviewerName, review.Comment, review.Rating);
    }

    public static Review ToEntity(this CreateReviewRequest request, int movieId)
    {
        return new Review
        {
            ReviewerName = request.ReviewerName,
            Comment = request.Comment,
            Rating = request.Rating,
            MovieId = movieId
        };
    }
}
