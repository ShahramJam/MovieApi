using Microsoft.AspNetCore.Mvc;
using MovieApi.Data;
using MovieApi.DTOs.Reports;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MovieApi.Controllers;

[Route("api/reports")]
[ApiController]
public class ReportsController(IUnitOfWork unitOfWork) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    [HttpGet("movies/top5pergenre")]
    public async Task<ActionResult<IEnumerable<GenreTopMoviesDto>>> GetTopFiveMoviesPerGenre(CancellationToken ct = default)
    {
        return await _unitOfWork.Reports.GetTopFiveMoviesPerGenreAsync(ct);
    }

    [HttpGet("movies/average-ratings")]
    public async Task<ActionResult<IEnumerable<GenreAverageRatingDto>>> GetAverageRatingsPerGenre(CancellationToken ct = default)
    {
        return await _unitOfWork.Reports.GetAverageRatingsPerGenreAsync(ct);
    }

    [HttpGet("actors/most-active")]
    public async Task<ActionResult<IEnumerable<ActorActivityDto>>> GetMostActiveActors(CancellationToken ct = default)
    {
        return await _unitOfWork.Reports.GetMostActiveActorsAsync(ct);
    }

    [HttpGet("movies/longest-per-country")]
    public async Task<ActionResult<IEnumerable<CountryLongestMovieDto>>> GetLongestMoviesPerCountry(CancellationToken ct = default)
    {
        return await _unitOfWork.Reports.GetLongestMoviesPerCountryAsync(ct);
    }

    [HttpGet("movies/with-most-reviews")]
    public async Task<ActionResult<IEnumerable<MovieReviewCountDto>>> GetMoviesWithMostReviews(CancellationToken ct = default)
    {
        return await _unitOfWork.Reports.GetMoviesWithMostReviewsAsync(ct);
    }

    [HttpGet("genres/popular")]
    public async Task<ActionResult<IEnumerable<GenrePopularityDto>>> GetPopularGenres(CancellationToken ct = default)
    {
        return await _unitOfWork.Reports.GetPopularGenresAsync(ct);
    }
}