namespace MovieApi.DTOs.MovieDetails;

public record MovieDetailsResponse(
    int Id,
    string Synopsis,
    string Language,
    decimal Budget);
