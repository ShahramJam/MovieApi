using MovieApi.DTOs.Actors;
using System.Collections.Generic;

namespace MovieApi.DTOs.Movies;

public class MovieDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Duration { get; set; }
    public int DirectorId { get; set; }
    public string DirectorName { get; set; } = string.Empty;
    public int CountryId { get; set; }
    public string CountryName { get; set; } = string.Empty;
    public List<string> Genres { get; set; } = new();
    public List<ActorDto> Actors { get; set; } = new();
}
