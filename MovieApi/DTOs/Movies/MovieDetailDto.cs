using MovieApi.DTOs.Actors;
using MovieApi.DTOs.MovieDetails;
using MovieApi.DTOs.Reviews;
using System.Collections.Generic;

namespace MovieApi.DTOs.Movies;

public class MovieDetailDto : MovieDto
{
    public MovieDetailsDto? MovieDetails { get; set; }
    public List<ReviewDto> Reviews { get; set; } = new();
    public List<ActorDto> Actors { get; set; } = new();
}
