using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApi.Data;
using MovieApi.DTOs.Reports;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApi.Controllers;

[Route("api/reports")]
[ApiController]
public class ReportsController : ControllerBase
{
    private readonly MovieDbContext _context;

    public ReportsController(MovieDbContext context)
    {
        _context = context;
    }

    [HttpGet("movies/top5pergenre")]
    public async Task<ActionResult<IEnumerable<GenreTopMoviesDto>>> GetTopFiveMoviesPerGenre()
    {
        var genres = await _context.Genres
            .AsNoTracking()
            .OrderBy(genre => genre.Name)
            .Select(genre => new { genre.Id, genre.Name })
            .ToListAsync();

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
            .ToListAsync();

        var result = genres.Select(genre => new GenreTopMoviesDto
        {
            GenreId = genre.Id,
            Genre = genre.Name,
            Movies = ratedMovies
                .Where(movie => movie.GenreId == genre.Id)
                .OrderByDescending(movie => movie.AverageRating)
                .ThenBy(movie => movie.Title)
                .Take(5)
                .Select(movie => new MovieRatingDto
                {
                    MovieId = movie.MovieId,
                    Title = movie.Title,
                    Year = movie.Year,
                    AverageRating = movie.AverageRating,
                    ReviewCount = movie.ReviewCount
                })
                .ToList()
        }).ToList();

        return result;
    }

    [HttpGet("movies/average-ratings")]
    public async Task<ActionResult<IEnumerable<GenreAverageRatingDto>>> GetAverageRatingsPerGenre()
    {
        var result = await _context.Genres
            .AsNoTracking()
            .OrderBy(genre => genre.Name)
            .Select(genre => new GenreAverageRatingDto
            {
                GenreId = genre.Id,
                Genre = genre.Name,
                AverageRating = genre.Movies
                    .SelectMany(movie => movie.Reviews)
                    .Select(review => (double?)review.Rating)
                    .Average(),
                ReviewCount = genre.Movies
                    .SelectMany(movie => movie.Reviews)
                    .Count()
            })
            .ToListAsync();

        return result;
    }

    [HttpGet("actors/most-active")]
    public async Task<ActionResult<IEnumerable<ActorActivityDto>>> GetMostActiveActors()
    {
        var actors = await _context.Actors
            .AsNoTracking()
            .Select(actor => new ActorActivityDto
            {
                ActorId = actor.Id,
                Name = actor.Name,
                MovieCount = actor.MovieActors.Count
            })
            .ToListAsync();

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

    [HttpGet("movies/longest-per-country")]
    public async Task<ActionResult<IEnumerable<CountryLongestMovieDto>>> GetLongestMoviesPerCountry()
    {
        var longestDurations = _context.Movies
            .GroupBy(movie => movie.CountryId)
            .Select(group => new
            {
                CountryId = group.Key,
                Duration = group.Max(movie => movie.Duration)
            });

        var result = await _context.Movies
            .AsNoTracking()
            .Join(
                longestDurations,
                movie => new { movie.CountryId, movie.Duration },
                longest => new { longest.CountryId, longest.Duration },
                (movie, longest) => new CountryLongestMovieDto
                {
                    CountryId = movie.CountryId,
                    Country = movie.Country.Name,
                    MovieId = movie.Id,
                    Title = movie.Title,
                    Year = movie.Year,
                    Duration = movie.Duration
                })
            .OrderBy(movie => movie.Country)
            .ThenBy(movie => movie.Title)
            .ToListAsync();

        return result;
    }

    [HttpGet("movies/with-most-reviews")]
    public async Task<ActionResult<IEnumerable<MovieReviewCountDto>>> GetMoviesWithMostReviews()
    {
        var movies = await _context.Movies
            .AsNoTracking()
            .Select(movie => new MovieReviewCountDto
            {
                MovieId = movie.Id,
                Title = movie.Title,
                Year = movie.Year,
                ReviewCount = movie.Reviews.Count
            })
            .ToListAsync();

        if (movies.Count == 0)
        {
            return new List<MovieReviewCountDto>();
        }

        var mostReviews = movies.Max(movie => movie.ReviewCount);
        return movies
            .Where(movie => movie.ReviewCount == mostReviews)
            .OrderBy(movie => movie.Title)
            .ToList();
    }

    [HttpGet("genres/popular")]
    public async Task<ActionResult<IEnumerable<GenrePopularityDto>>> GetPopularGenres()
    {
        var result = await _context.Genres
            .AsNoTracking()
            .Select(genre => new GenrePopularityDto
            {
                GenreId = genre.Id,
                Genre = genre.Name,
                MovieCount = genre.Movies.Count
            })
            .OrderByDescending(genre => genre.MovieCount)
            .ThenBy(genre => genre.Genre)
            .ToListAsync();

        return result;
    }
}