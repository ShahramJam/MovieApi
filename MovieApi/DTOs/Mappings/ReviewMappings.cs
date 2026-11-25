using MovieApi.DTOs.Reviews;
using MovieApi.Models;

namespace MovieApi.DTOs.Mappings;

public static class ReviewMappings
{
    public static ReviewDto ToDto(this Review review)
    {
        return new ReviewDto(review.Id, review.ReviewerName, review.Comment, review.Rating);
    }

    public static Review ToEntity(this ReviewDto reviewDto, int movieId)
    {
        return new Review
        {
            ReviewerName = reviewDto.ReviewerName,
            Comment = reviewDto.Comment,
            Rating = reviewDto.Rating,
            MovieId = movieId
        };
    }
}
