using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MovieApi.DTOs.Actors;

public record ActorDto(
    int Id,
    [Required, StringLength(150, MinimumLength = 1)] string Name,
    [Range(1888, 2100)] int BirthYear,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? Role = null);
