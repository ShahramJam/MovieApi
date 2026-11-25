using System.ComponentModel.DataAnnotations;

namespace MovieApi.DTOs.MovieDetails;

public record MovieDetailsDto(
    int Id,
    [Required, StringLength(4000)] string Synopsis,
    [Required, StringLength(100)] string Language,
    [Range(typeof(decimal), "0", "9999999999999999.99")] decimal Budget);
