using MovieApi.DTOs.Actors;
using MovieApi.DTOs.MovieActors;
using MovieApi.Models;
using System.Collections.Generic;
using System.Linq;

namespace MovieApi.DTOs.Mappings;

public static class MovieActorMappings
{
    public static ActorResponse ToResponse(this MovieActor movieActor)
    {
        return new ActorResponse(
            movieActor.Actor.Id,
            movieActor.Actor.Name,
            movieActor.Actor.BirthYear,
            movieActor.Role);
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
        this AddMovieActorRequest request,
        int movieId,
        Actor actor)
    {
        return new MovieActor
        {
            MovieId = movieId,
            ActorId = actor.Id,
            Actor = actor,
            Role = request.Role
        };
    }

    public static MovieActor ToEntity(
        this AddMovieActorRoleRequest request,
        int movieId,
        Actor actor)
    {
        return new MovieActor
        {
            MovieId = movieId,
            ActorId = actor.Id,
            Actor = actor,
            Role = request.Role
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
