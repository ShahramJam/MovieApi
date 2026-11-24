using System.Collections.Generic;
using System.Linq;
using MovieApi.DTOs;
using MovieApi.Models;

namespace MovieApi.DTOs.Mappings;

public static class MovieActorMappings
{
    public static ActorDto ToDto(this MovieActor movieActor)
    {
        return new ActorDto
        {
            Id = movieActor.Actor.Id,
            Name = movieActor.Actor.Name,
            BirthYear = movieActor.Actor.BirthYear,
            Role = movieActor.Role
        };
    }

    public static MovieActor ToMovieActor(this Actor actor, string role = "Actor")
    {
        return new MovieActor
        {
            Actor = actor,
            Role = role
        };
    }

    public static MovieActor ToEntity(
        this MovieActorCreateDto movieActorDto,
        int movieId,
        Actor actor)
    {
        return new MovieActor
        {
            MovieId = movieId,
            ActorId = actor.Id,
            Actor = actor,
            Role = movieActorDto.Role
        };
    }

    public static void SyncActors(this Movie movie, IEnumerable<Actor> actors)
    {
        var requestedActorIds = actors.Select(actor => actor.Id).ToHashSet();
        var existingActorIds = movie.MovieActors
            .Select(movieActor => movieActor.ActorId)
            .ToHashSet();

        foreach (var movieActor in movie.MovieActors
            .Where(movieActor => !requestedActorIds.Contains(movieActor.ActorId))
            .ToList())
        {
            movie.MovieActors.Remove(movieActor);
        }

        foreach (var actor in actors)
        {
            if (!existingActorIds.Contains(actor.Id))
            {
                movie.MovieActors.Add(actor.ToMovieActor());
            }
        }
    }
}
