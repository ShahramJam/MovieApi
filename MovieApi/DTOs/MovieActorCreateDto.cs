using System.ComponentModel.DataAnnotations;

namespace MovieApi.DTOs;

public class MovieActorCreateDto
{
    [Range(1, int.MaxValue)]
    public int ActorId { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Role { get; set; } = string.Empty;
}