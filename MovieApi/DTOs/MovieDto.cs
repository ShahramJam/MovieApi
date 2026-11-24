using System.Collections.Generic;

namespace MovieApi.DTOs;

public class MovieDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Duration { get; set; }
    public List<string> Genres { get; set; } = new();
}
