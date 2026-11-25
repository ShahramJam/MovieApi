using MovieApi.DTOs.Actors;
using System.Collections.Generic;

namespace MovieApi.DTOs.Movies;

public record MovieResponse(
    int Id,
    string Title,
    int Year,
    int Duration,
    int DirectorId,
    string DirectorName,
    int CountryId,
    string CountryName,
    List<string> Genres,
    List<ActorResponse> Actors);
