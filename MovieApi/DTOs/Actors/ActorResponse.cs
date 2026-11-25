using System.Text.Json.Serialization;

namespace MovieApi.DTOs.Actors;

public record ActorResponse(
    int Id,
    string Name,
    int BirthYear,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? Role = null);
