using MovieApi.DTOs.MovieDetails;
using MovieDetailsEntity = MovieApi.Models.MovieDetails;

namespace MovieApi.DTOs.Mappings;

public static class MovieDetailsMappings
{
    public static MovieDetailsDto ToDto(this MovieDetailsEntity movieDetails)
    {
        return new MovieDetailsDto
        {
            Id = movieDetails.Id,
            Synopsis = movieDetails.Synopsis,
            Language = movieDetails.Language,
            Budget = movieDetails.Budget
        };
    }
}
