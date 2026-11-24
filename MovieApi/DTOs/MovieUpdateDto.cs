using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieApi.DTOs;

public class MovieUpdateDto
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [Range(1888, 2100)]
    public int Year { get; set; }

    [Range(1, 1000)]
    public int Duration { get; set; }

    [Required]
    [MinLength(1)]
    public List<int> GenreIds { get; set; } = new();

    public List<int> ActorIds { get; set; } = new();
}
