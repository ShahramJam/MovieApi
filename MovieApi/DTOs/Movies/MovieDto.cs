using MovieApi.DTOs.Actors;
using System.Collections.Generic;

namespace MovieApi.DTOs.Movies;

public record MovieDto(
    int Id,
    string Title,
    int Year,
    int Duration,
    int DirectorId,
    string DirectorName,
    int CountryId,
    string CountryName,
    List<string> Genres,
    List<ActorDto> Actors);
