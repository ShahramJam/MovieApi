using Microsoft.EntityFrameworkCore;
using MovieApi.Data;
using MovieApi.DTOs.Reports;
using MovieApi.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MovieApi.Repositories;

public sealed class ReportRepository(MovieDbContext context) : IReportRepository
{
    private readonly MovieDbContext _context = context;

    public async Task<List<GenreTopMoviesResponse>> GetTopFiveMoviesPerGenreAsync(CancellationToken ct = default)
    {
        var genres = await _context.Genres
            .AsNoTracking()
            .OrderBy(genre => genre.Name)
            .Select(genre => new { genre.Id, genre.Name })
            .ToListAsync(ct);

        var movieRatings = _context.Reviews
            .AsNoTracking()
            .GroupBy(review => review.MovieId)
            .Select(group => new
            {
                MovieId = group.Key,
                AverageRating = group.Average(review => (double)review.Rating),
                ReviewCount = group.Count()
            });

        var ratedMovies = await _context.Movies
            .AsNoTracking()
            .SelectMany(
                movie => movie.Genres,
                (movie, genre) => new { Movie = movie, Genre = genre })
            .Join(
                movieRatings,
                item => item.Movie.Id,
                rating => rating.MovieId,
                (item, rating) => new
                {
                    GenreId = item.Genre.Id,
                    MovieId = item.Movie.Id,
                    item.Movie.Title,
                    item.Movie.Year,
                    rating.AverageRating,
                    rating.ReviewCount
                })
            .ToListAsync(ct);

        return genres.Select(genre => new GenreTopMoviesResponse(
            genre.Id,
            genre.Name,
            ratedMovies
                .Where(movie => movie.GenreId == genre.Id)
                .OrderByDescending(movie => movie.AverageRating)
                .ThenBy(movie => movie.Title)
                .Take(5)
                .Select(movie => new MovieRatingResponse(
                    movie.MovieId,
                    movie.Title,
                    movie.Year,
                    movie.AverageRating,
                    movie.ReviewCount))
                .ToList())).ToList();
    }

    public Task<List<GenreAverageRatingResponse>> GetAverageRatingsPerGenreAsync(CancellationToken ct = default)
    {
        return _context.Genres
            .AsNoTracking()
            .OrderBy(genre => genre.Name)
            .Select(genre => new GenreAverageRatingResponse(
                genre.Id,
                genre.Name,
                genre.Movies
                    .SelectMany(movie => movie.Reviews)
                    .Select(review => (double?)review.Rating)
                    .Average(),
                genre.Movies
                    .SelectMany(movie => movie.Reviews)
                    .Count()))
            .ToListAsync(ct);
    }

    public async Task<List<ActorActivityResponse>> GetMostActiveActorsAsync(CancellationToken ct = default)
    {
        var actors = await _context.Actors
            .AsNoTracking()
            .Select(actor => new ActorActivityResponse(actor.Id, actor.Name, actor.MovieActors.Count))
            .ToListAsync(ct);

        if (actors.Count == 0)
        {
            return actors;
        }

        var highestMovieCount = actors.Max(actor => actor.MovieCount);
        return actors
            .Where(actor => actor.MovieCount == highestMovieCount)
            .OrderBy(actor => actor.Name)
            .ToList();
    }

    public async Task<List<CountryLongestMovieResponse>> GetLongestMoviesPerCountryAsync(CancellationToken ct = default)
    {
        var longestDurations = _context.Movies
            .GroupBy(movie => movie.CountryId)
            .Select(group => new
            {
                CountryId = group.Key,
                Duration = group.Max(movie => movie.Duration)
            });

        return await _context.Movies
            .AsNoTracking()
            .Join(
                longestDurations,
                movie => new { movie.CountryId, movie.Duration },
                longest => new { longest.CountryId, longest.Duration },
                (movie, longest) => new CountryLongestMovieResponse(
                    movie.CountryId,
                    movie.Country.Name,
                    movie.Id,
                    movie.Title,
                    movie.Year,
                    movie.Duration))
            .OrderBy(movie => movie.Country)
            .ThenBy(movie => movie.Title)
            .ToListAsync(ct);
    }

    public async Task<List<MovieReviewCountResponse>> GetMoviesWithMostReviewsAsync(CancellationToken ct = default)
    {
        var movies = await _context.Movies
            .AsNoTracking()
            .Select(movie => new MovieReviewCountResponse(
                movie.Id,
                movie.Title,
                movie.Year,
                movie.Reviews.Count))
            .ToListAsync(ct);

        if (movies.Count == 0)
        {
            return new List<MovieReviewCountResponse>();
        }

        var mostReviews = movies.Max(movie => movie.ReviewCount);
        return movies
            .Where(movie => movie.ReviewCount == mostReviews)
            .OrderBy(movie => movie.Title)
            .ToList();
    }

    public Task<List<GenrePopularityResponse>> GetPopularGenresAsync(CancellationToken ct = default)
    {
        return _context.Genres
            .AsNoTracking()
            .Select(genre => new GenrePopularityResponse(genre.Id, genre.Name, genre.Movies.Count))
            .OrderByDescending(genre => genre.MovieCount)
            .ThenBy(genre => genre.Genre)
            .ToListAsync(ct);
    }
}