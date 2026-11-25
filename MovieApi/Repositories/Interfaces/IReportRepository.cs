using MovieApi.DTOs.Reports;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MovieApi.Repositories.Interfaces;

public interface IReportRepository
{
    Task<List<GenreTopMoviesDto>> GetTopFiveMoviesPerGenreAsync(CancellationToken ct = default);
    Task<List<GenreAverageRatingDto>> GetAverageRatingsPerGenreAsync(CancellationToken ct = default);
    Task<List<ActorActivityDto>> GetMostActiveActorsAsync(CancellationToken ct = default);
    Task<List<CountryLongestMovieDto>> GetLongestMoviesPerCountryAsync(CancellationToken ct = default);
    Task<List<MovieReviewCountDto>> GetMoviesWithMostReviewsAsync(CancellationToken ct = default);
    Task<List<GenrePopularityDto>> GetPopularGenresAsync(CancellationToken ct = default);
}