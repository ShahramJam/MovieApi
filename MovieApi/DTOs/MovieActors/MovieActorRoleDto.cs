using System.ComponentModel.DataAnnotations;

namespace MovieApi.DTOs.MovieActors;

public record MovieActorRoleDto(
    [Required, StringLength(100, MinimumLength = 1)] string Role);
