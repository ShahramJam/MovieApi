using System.Collections.Generic;

namespace MovieApi.Models;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Year { get; set; }
    public int DirectorId { get; set; }
    public Director Director { get; set; } = null!;
    public int CountryId { get; set; }
    public Country Country { get; set; } = null!;
    public ICollection<Genre> Genres { get; set; } = new List<Genre>();
    public int Duration { get; set; }
    public MovieDetails? MovieDetails { get; set; }
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
}
