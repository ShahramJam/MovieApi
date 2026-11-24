using System.Collections.Generic;

namespace MovieApi.DTOs;

public class MovieDetailDto : MovieDto
{
    public MovieDetailsDto? MovieDetails { get; set; }
    public List<ReviewDto> Reviews { get; set; } = new();
    public List<ActorDto> Actors { get; set; } = new();
}
