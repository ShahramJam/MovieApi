using MovieApi.DTOs.Actors;
using MovieApi.Models;

namespace MovieApi.DTOs.Mappings;

public static class ActorMappings
{
    public static ActorResponse ToResponse(this Actor actor)
    {
        return new ActorResponse(actor.Id, actor.Name, actor.BirthYear);
    }

    public static Actor ToEntity(this CreateActorRequest request)
    {
        return new Actor
        {
            Name = request.Name,
            BirthYear = request.BirthYear
        };
    }

    public static void UpdateEntity(this UpdateActorRequest request, Actor actor)
    {
        actor.Name = request.Name;
        actor.BirthYear = request.BirthYear;
    }
}
