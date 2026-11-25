using System.ComponentModel.DataAnnotations;

namespace MovieApi.DTOs.MovieActors;

public record MovieActorCreateDto(
    [Range(1, int.MaxValue)] int ActorId,
    [Required, StringLength(100, MinimumLength = 1)] string Role);
