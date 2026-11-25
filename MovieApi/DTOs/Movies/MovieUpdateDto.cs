using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieApi.DTOs.Movies;

public record MovieUpdateDto(
    [Required, StringLength(200, MinimumLength = 1)] string Title,
    [Range(1888, 2100)] int Year,
    [Range(1, 1000)] int Duration,
    [Range(1, int.MaxValue)] int DirectorId,
    [Range(1, int.MaxValue)] int CountryId,
    [Required, MinLength(1)] List<int> GenreIds,
    List<int>? ActorIds = null);
