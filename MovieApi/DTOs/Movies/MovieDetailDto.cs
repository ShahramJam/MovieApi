using MovieApi.DTOs.Actors;
using MovieApi.DTOs.MovieDetails;
using MovieApi.DTOs.Reviews;
using System.Collections.Generic;

namespace MovieApi.DTOs.Movies;

public record MovieDetailDto(
    int Id,
    string Title,
    int Year,
    int Duration,
    int DirectorId,
    string DirectorName,
    int CountryId,
    string CountryName,
    List<string> Genres,
    List<ActorDto> Actors,
    MovieDetailsDto? MovieDetails,
    List<ReviewDto> Reviews)
    : MovieDto(Id, Title, Year, Duration, DirectorId, DirectorName, CountryId, CountryName, Genres, Actors);
