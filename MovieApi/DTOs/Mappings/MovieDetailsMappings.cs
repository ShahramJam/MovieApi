using MovieApi.DTOs;
using MovieApi.Models;

namespace MovieApi.DTOs.Mappings;

public static class MovieDetailsMappings
{
    public static MovieDetailsDto ToDto(this MovieDetails movieDetails)
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
