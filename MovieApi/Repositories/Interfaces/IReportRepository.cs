using MovieApi.DTOs.Reports;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MovieApi.Repositories.Interfaces;

public interface IReportRepository
{
    Task<List<GenreTopMoviesResponse>> GetTopFiveMoviesPerGenreAsync(CancellationToken ct = default);
    Task<List<GenreAverageRatingResponse>> GetAverageRatingsPerGenreAsync(CancellationToken ct = default);
    Task<List<ActorActivityResponse>> GetMostActiveActorsAsync(CancellationToken ct = default);
    Task<List<CountryLongestMovieResponse>> GetLongestMoviesPerCountryAsync(CancellationToken ct = default);
    Task<List<MovieReviewCountResponse>> GetMoviesWithMostReviewsAsync(CancellationToken ct = default);
    Task<List<GenrePopularityResponse>> GetPopularGenresAsync(CancellationToken ct = default);
}