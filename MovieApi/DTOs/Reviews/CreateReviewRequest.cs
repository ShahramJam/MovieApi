
using System.ComponentModel.DataAnnotations;

namespace MovieApi.DTOs.Reviews;

public record CreateReviewRequest(
    [Required, StringLength(100, MinimumLength = 1)] string ReviewerName,
    [Required, StringLength(2000, MinimumLength = 1)] string Comment,
    [Range(1, 5)] int Rating);
