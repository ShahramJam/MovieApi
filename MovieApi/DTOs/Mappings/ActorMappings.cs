using MovieApi.DTOs;
using MovieApi.Models;

namespace MovieApi.DTOs.Mappings;

public static class ActorMappings
{
    public static ActorDto ToDto(this Actor actor)
    {
        return new ActorDto
        {
            Id = actor.Id,
            Name = actor.Name,
            BirthYear = actor.BirthYear
        };
    }

    public static Actor ToEntity(this ActorDto actorDto)
    {
        return new Actor
        {
            Name = actorDto.Name,
            BirthYear = actorDto.BirthYear
        };
    }

    public static void UpdateEntity(this ActorDto actorDto, Actor actor)
    {
        actor.Name = actorDto.Name;
        actor.BirthYear = actorDto.BirthYear;
    }
}
