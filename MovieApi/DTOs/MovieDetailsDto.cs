using System.ComponentModel.DataAnnotations;

namespace MovieApi.DTOs;

public class MovieDetailsDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(4000)]
    public string Synopsis { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Language { get; set; } = string.Empty;

    [Range(typeof(decimal), "0", "9999999999999999.99")]
    public decimal Budget { get; set; }
}
