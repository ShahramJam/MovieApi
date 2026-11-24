using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MovieApi.DTOs;

public class ActorDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(150, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [Range(1888, 2100)]
    public int BirthYear { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Role { get; set; }
}
