using System.Collections.Generic;
using System.Linq;
using MovieApi.DTOs;
using MovieApi.Models;

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

    public static MovieDetailDto ToDetailDto(this Movie movie)
    {
        return new MovieDetailDto
        {
            Id = movie.Id,
            Title = movie.Title,
            Year = movie.Year,
            Duration = movie.Duration,
            Genres = movie.Genres.Select(genre => genre.Name).OrderBy(name => name).ToList(),
            MovieDetails = movie.MovieDetails?.ToDto(),
            Reviews = movie.Reviews
                .OrderBy(review => review.Id)
                .Select(review => review.ToDto())
                .ToList(),
            Actors = movie.MovieActors
                .OrderBy(movieActor => movieActor.Actor.Name)
                .Select(movieActor => movieActor.ToDto())
                .ToList()
        };
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
