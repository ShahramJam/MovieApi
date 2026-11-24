using MovieApi.DTOs;
using MovieApi.Models;

namespace MovieApi.DTOs.Mappings;

public static class ReviewMappings
{
    public static ReviewDto ToDto(this Review review)
    {
        return new ReviewDto
        {
            Id = review.Id,
            ReviewerName = review.ReviewerName,
            Comment = review.Comment,
            Rating = review.Rating
        };
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
