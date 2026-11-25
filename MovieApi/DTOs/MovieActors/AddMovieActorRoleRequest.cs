using System.ComponentModel.DataAnnotations;

namespace MovieApi.DTOs.MovieActors;

public record AddMovieActorRoleRequest(
    [Required, StringLength(100, MinimumLength = 1)] string Role);
