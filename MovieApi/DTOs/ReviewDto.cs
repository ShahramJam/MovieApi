using System.ComponentModel.DataAnnotations;

namespace MovieApi.DTOs;

public class ReviewDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string ReviewerName { get; set; } = string.Empty;

    [Required]
    [StringLength(2000, MinimumLength = 1)]
    public string Comment { get; set; } = string.Empty;

    [Range(1, 5)]
    public int Rating { get; set; }
}
