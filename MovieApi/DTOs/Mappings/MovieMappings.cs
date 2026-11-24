using MovieApi.Models;
using System.Collections.Generic;
using System.Linq;

namespace MovieApi.DTOs.Mappings;

public static class MovieMappings
{
    public static MovieDto ToDto(this Movie movie, bool includeActors = false)
    {
        return new MovieDto
        {
            Id = movie.Id,
            Title = movie.Title,
            Year = movie.Year,
            Duration = movie.Duration,
            Genres = movie.Genres.Select(genre => genre.Name).OrderBy(name => name).ToList(),
            Actors = includeActors
                ? movie.MovieActors
                    .OrderBy(movieActor => movieActor.Actor.Name)
                    .Select(movieActor => movieActor.ToDto())
                    .ToList()
                : new List<ActorDto>()
        };
    }

    public static IQueryable<MovieDetailDto> SelectDetailDto(this IQueryable<Movie> movies)
    {
        return movies.Select(movie => new MovieDetailDto
        {
            Id = movie.Id,
            Title = movie.Title,
            Year = movie.Year,
            Duration = movie.Duration,
            Genres = movie.Genres
                .Select(genre => genre.Name)
                .OrderBy(name => name)
                .ToList(),
            MovieDetails = movie.MovieDetails == null ? null : new MovieDetailsDto
            {
                Id = movie.MovieDetails.Id,
                Synopsis = movie.MovieDetails.Synopsis,
                Language = movie.MovieDetails.Language,
                Budget = movie.MovieDetails.Budget
            },
            Reviews = movie.Reviews
                .OrderBy(review => review.Id)
                .Select(review => new ReviewDto
                {
                    Id = review.Id,
                    ReviewerName = review.ReviewerName,
                    Comment = review.Comment,
                    Rating = review.Rating
                })
                .ToList(),
            Actors = movie.MovieActors
                .OrderBy(movieActor => movieActor.Actor.Name)
                .Select(movieActor => new ActorDto
                {
                    Id = movieActor.Actor.Id,
                    Name = movieActor.Actor.Name,
                    BirthYear = movieActor.Actor.BirthYear,
                    Role = movieActor.Role
                })
                .ToList()
        });
    }

    public static Movie ToEntity(
        this MovieCreateDto movieDto,
        IEnumerable<Genre> genres,
        IEnumerable<Actor> actors)
    {
        return new Movie
        {
            Title = movieDto.Title,
            Year = movieDto.Year,
            Duration = movieDto.Duration,
            Genres = genres.ToList(),
            MovieActors = actors.Select(actor => actor.ToMovieActor()).ToList()
        };
    }

    public static void UpdateEntity(
        this MovieUpdateDto movieDto,
        Movie movie,
        IEnumerable<Genre> genres,
        IEnumerable<Actor> actors)
    {
        movie.Title = movieDto.Title;
        movie.Year = movieDto.Year;
        movie.Duration = movieDto.Duration;
        movie.Genres.Clear();

        foreach (var genre in genres)
        {
            movie.Genres.Add(genre);
        }

        movie.SyncActors(actors);
    }
}
