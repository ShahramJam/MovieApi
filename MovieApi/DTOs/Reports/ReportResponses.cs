using System.Collections.Generic;

namespace MovieApi.DTOs.Reports;

public sealed record GenreTopMoviesResponse(int GenreId, string Genre, List<MovieRatingResponse> Movies);

public sealed record MovieRatingResponse(
    int MovieId,
    string Title,
    int Year,
    double AverageRating,
    int ReviewCount);

public sealed record GenreAverageRatingResponse(
    int GenreId,
    string Genre,
    double? AverageRating,
    int ReviewCount);

public sealed record ActorActivityResponse(int ActorId, string Name, int MovieCount);

public sealed record CountryLongestMovieResponse(
    int CountryId,
    string Country,
    int MovieId,
    string Title,
    int Year,
    int Duration);

public sealed record MovieReviewCountResponse(int MovieId, string Title, int Year, int ReviewCount);

public sealed record GenrePopularityResponse(int GenreId, string Genre, int MovieCount);
