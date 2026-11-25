using MovieApi.DTOs.Actors;
using MovieApi.DTOs.MovieDetails;
using MovieApi.DTOs.Reviews;
using System.Collections.Generic;

namespace MovieApi.DTOs.Movies;

public record MovieDetailResponse(
    int Id,
    string Title,
    int Year,
    int Duration,
    int DirectorId,
    string DirectorName,
    int CountryId,
    string CountryName,
    List<string> Genres,
    List<ActorResponse> Actors,
    MovieDetailsResponse? MovieDetails,
    List<ReviewResponse> Reviews)
    : MovieResponse(Id, Title, Year, Duration, DirectorId, DirectorName, CountryId, CountryName, Genres, Actors);
