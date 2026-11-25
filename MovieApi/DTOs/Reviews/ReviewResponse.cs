namespace MovieApi.DTOs.Reviews;

public record ReviewResponse(int Id, string ReviewerName, string Comment, int Rating);