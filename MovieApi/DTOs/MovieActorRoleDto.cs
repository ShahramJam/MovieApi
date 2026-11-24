using System.ComponentModel.DataAnnotations;

namespace MovieApi.DTOs;

public class MovieActorRoleDto
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Role { get; set; } = string.Empty;
}