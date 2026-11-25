using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
    public async Task<Ok<IReadOnlyList<GenreTopMoviesResponse>>> GetTopFiveMoviesPerGenre(CancellationToken ct = default)
    {
        return TypedResults.Ok<IReadOnlyList<GenreTopMoviesResponse>>(
            await _unitOfWork.Reports.GetTopFiveMoviesPerGenreAsync(ct));
    }

    [HttpGet("movies/average-ratings")]
    public async Task<Ok<IReadOnlyList<GenreAverageRatingResponse>>> GetAverageRatingsPerGenre(CancellationToken ct = default)
    {
        return TypedResults.Ok<IReadOnlyList<GenreAverageRatingResponse>>(
            await _unitOfWork.Reports.GetAverageRatingsPerGenreAsync(ct));
    }

    [HttpGet("actors/most-active")]
    public async Task<Ok<IReadOnlyList<ActorActivityResponse>>> GetMostActiveActors(CancellationToken ct = default)
    {
        return TypedResults.Ok<IReadOnlyList<ActorActivityResponse>>(
            await _unitOfWork.Reports.GetMostActiveActorsAsync(ct));
    }

    [HttpGet("movies/longest-per-country")]
    public async Task<Ok<IReadOnlyList<CountryLongestMovieResponse>>> GetLongestMoviesPerCountry(CancellationToken ct = default)
    {
        return TypedResults.Ok<IReadOnlyList<CountryLongestMovieResponse>>(
            await _unitOfWork.Reports.GetLongestMoviesPerCountryAsync(ct));
    }

    [HttpGet("movies/with-most-reviews")]
    public async Task<Ok<IReadOnlyList<MovieReviewCountResponse>>> GetMoviesWithMostReviews(CancellationToken ct = default)
    {
        return TypedResults.Ok<IReadOnlyList<MovieReviewCountResponse>>(
            await _unitOfWork.Reports.GetMoviesWithMostReviewsAsync(ct));
    }

    [HttpGet("genres/popular")]
    public async Task<Ok<IReadOnlyList<GenrePopularityResponse>>> GetPopularGenres(CancellationToken ct = default)
    {
        return TypedResults.Ok<IReadOnlyList<GenrePopularityResponse>>(
            await _unitOfWork.Reports.GetPopularGenresAsync(ct));
    }
}