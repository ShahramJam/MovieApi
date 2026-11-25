using System.Collections.Generic;

namespace MovieApi.DTOs.Reports;

public sealed record GenreTopMoviesDto(int GenreId, string Genre, List<MovieRatingDto> Movies);

public sealed record MovieRatingDto(
    int MovieId,
    string Title,
    int Year,
    double AverageRating,
    int ReviewCount);

public sealed record GenreAverageRatingDto(
    int GenreId,
    string Genre,
    double? AverageRating,
    int ReviewCount);

public sealed record ActorActivityDto(int ActorId, string Name, int MovieCount);

public sealed record CountryLongestMovieDto(
    int CountryId,
    string Country,
    int MovieId,
    string Title,
    int Year,
    int Duration);

public sealed record MovieReviewCountDto(int MovieId, string Title, int Year, int ReviewCount);

public sealed record GenrePopularityDto(int GenreId, string Genre, int MovieCount);
