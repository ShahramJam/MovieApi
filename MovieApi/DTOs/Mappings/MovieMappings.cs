using MovieApi.DTOs.Actors;
using MovieApi.DTOs.MovieDetails;
using MovieApi.DTOs.Movies;
using MovieApi.DTOs.Reviews;
using MovieApi.Models;
using System.Collections.Generic;
using System.Linq;

namespace MovieApi.DTOs.Mappings;

public static class MovieMappings
{
    public static MovieResponse ToResponse(this Movie movie, bool includeActors = false)
    {
        return new MovieResponse(
            movie.Id,
            movie.Title,
            movie.Year,
            movie.Duration,
            movie.DirectorId,
            movie.Director.Name,
            movie.CountryId,
            movie.Country.Name,
            movie.Genres.Select(genre => genre.Name).OrderBy(name => name).ToList(),
            includeActors
                ? movie.MovieActors
                    .OrderBy(movieActor => movieActor.Actor.Name)
                    .Select(movieActor => movieActor.ToResponse())
                    .ToList()
                : new List<ActorResponse>());
    }

    public static IQueryable<MovieDetailResponse> SelectDetailResponse(this IQueryable<Movie> movies)
    {
        return movies.Select(movie => new MovieDetailResponse(
            movie.Id,
            movie.Title,
            movie.Year,
            movie.Duration,
            movie.DirectorId,
            movie.Director.Name,
            movie.CountryId,
            movie.Country.Name,
            movie.Genres
                .Select(genre => genre.Name)
                .OrderBy(name => name)
                .ToList(),
            movie.MovieActors
                .OrderBy(movieActor => movieActor.Actor.Name)
                .Select(movieActor => new ActorResponse(
                    movieActor.Actor.Id,
                    movieActor.Actor.Name,
                    movieActor.Actor.BirthYear,
                    movieActor.Role))
                .ToList(),
            movie.MovieDetails == null
                ? null
                : new MovieDetailsResponse(
                    movie.MovieDetails.Id,
                    movie.MovieDetails.Synopsis,
                    movie.MovieDetails.Language,
                    movie.MovieDetails.Budget),
            movie.Reviews
                .OrderBy(review => review.Id)
                .Select(review => new ReviewResponse(
                    review.Id,
                    review.ReviewerName,
                    review.Comment,
                    review.Rating))
                .ToList()));
    }

    public static Movie ToEntity(
        this CreateMovieRequest request,
        Director director,
        Country country,
        IEnumerable<Genre> genres,
        IEnumerable<Actor> actors)
    {
        return new Movie
        {
            Title = request.Title,
            Year = request.Year,
            Duration = request.Duration,
            DirectorId = director.Id,
            Director = director,
            CountryId = country.Id,
            Country = country,
            Genres = genres.ToList(),
            MovieActors = actors.Select(actor => actor.ToMovieActor()).ToList()
        };
    }

    public static void UpdateEntity(
        this UpdateMovieRequest request,
        Movie movie,
        Director director,
        Country country,
        IEnumerable<Genre> genres,
        IEnumerable<Actor> actors)
    {
        movie.Title = request.Title;
        movie.Year = request.Year;
        movie.Duration = request.Duration;
        movie.DirectorId = director.Id;
        movie.Director = director;
        movie.CountryId = country.Id;
        movie.Country = country;
        movie.Genres.Clear();

        foreach (var genre in genres)
        {
            movie.Genres.Add(genre);
        }

        movie.SyncActors(actors);
    }
}
