using System.ComponentModel.DataAnnotations;

namespace MovieApi.DTOs;

public class ActorUpdateDto
{
    [Required]
    [StringLength(150, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [Range(1888, 2100)]
    public int BirthYear { get; set; }
}
