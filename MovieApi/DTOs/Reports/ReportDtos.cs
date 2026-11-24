using System.Collections.Generic;

namespace MovieApi.DTOs.Reports;

public sealed class GenreTopMoviesDto
{
    public int GenreId { get; set; }
    public string Genre { get; set; } = string.Empty;
    public List<MovieRatingDto> Movies { get; set; } = new();
}

public sealed class MovieRatingDto
{
    public int MovieId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Year { get; set; }
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
}

public sealed class GenreAverageRatingDto
{
    public int GenreId { get; set; }
    public string Genre { get; set; } = string.Empty;
    public double? AverageRating { get; set; }
    public int ReviewCount { get; set; }
}

public sealed class ActorActivityDto
{
    public int ActorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MovieCount { get; set; }
}

public sealed class CountryLongestMovieDto
{
    public int CountryId { get; set; }
    public string Country { get; set; } = string.Empty;
    public int MovieId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Duration { get; set; }
}

public sealed class MovieReviewCountDto
{
    public int MovieId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Year { get; set; }
    public int ReviewCount { get; set; }
}

public sealed class GenrePopularityDto
{
    public int GenreId { get; set; }
    public string Genre { get; set; } = string.Empty;
    public int MovieCount { get; set; }
}