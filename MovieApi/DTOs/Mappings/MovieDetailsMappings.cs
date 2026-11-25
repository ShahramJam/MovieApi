using MovieApi.DTOs.MovieDetails;
using MovieDetailsEntity = MovieApi.Models.MovieDetails;

namespace MovieApi.DTOs.Mappings;

public static class MovieDetailsMappings
{
    public static MovieDetailsDto ToDto(this MovieDetailsEntity movieDetails)
    {
        return new MovieDetailsDto(
            movieDetails.Id,
            movieDetails.Synopsis,
            movieDetails.Language,
            movieDetails.Budget);
    }
}
