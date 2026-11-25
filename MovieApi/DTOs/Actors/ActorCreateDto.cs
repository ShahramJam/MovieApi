using System.ComponentModel.DataAnnotations;

namespace MovieApi.DTOs.Actors;

public record ActorCreateDto(
    [Required, StringLength(150, MinimumLength = 1)] string Name,
    [Range(1888, 2100)] int BirthYear);
