using MovieApi.DTOs.MovieDetails;
using MovieDetailsEntity = MovieApi.Models.MovieDetails;

namespace MovieApi.DTOs.Mappings;

public static class MovieDetailsMappings
{
    public static MovieDetailsResponse ToResponse(this MovieDetailsEntity movieDetails)
    {
        return new MovieDetailsResponse(
            movieDetails.Id,
            movieDetails.Synopsis,
            movieDetails.Language,
            movieDetails.Budget);
    }
}
