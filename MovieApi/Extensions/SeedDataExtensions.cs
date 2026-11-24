using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MovieApi.Data;
using MovieApi.Models;

namespace MovieApi.Extensions;

public static class SeedDataExtensions
{
    public static void SeedData(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MovieDbContext>();

        dbContext.Database.Migrate();

        if (dbContext.Movies.Any())
        {
            return;
        }

        var action = new Genre { Name = "Action" };
        var crime = new Genre { Name = "Crime" };
        var drama = new Genre { Name = "Drama" };
        var scienceFiction = new Genre { Name = "Science Fiction" };

        var timRobbins = new Actor { Name = "Tim Robbins", BirthYear = 1958 };
        var morganFreeman = new Actor { Name = "Morgan Freeman", BirthYear = 1937 };
        var keanuReeves = new Actor { Name = "Keanu Reeves", BirthYear = 1964 };
        var laurenceFishburne = new Actor { Name = "Laurence Fishburne", BirthYear = 1961 };
        var christianBale = new Actor { Name = "Christian Bale", BirthYear = 1974 };
        var heathLedger = new Actor { Name = "Heath Ledger", BirthYear = 1979 };

        var movies = new List<Movie>
        {
            new()
            {
                Title = "The Shawshank Redemption",
                Year = 1994,
                Duration = 142,
                Genres = new List<Genre> { crime, drama },
                Actors = new List<Actor> { timRobbins, morganFreeman },
                MovieDetails = new MovieDetails
                {
                    Synopsis = "A long sentence tests the resilience and friendship of two inmates.",
                    Language = "English",
                    Budget = 25_000_000m
                },
                Reviews = new List<Review>
                {
                    new()
                    {
                        ReviewerName = "Alex",
                        Comment = "A moving drama with memorable performances.",
                        Rating = 5
                    },
                    new()
                    {
                        ReviewerName = "Sam",
                        Comment = "Thoughtful, patient, and rewarding.",
                        Rating = 4
                    }
                }
            },
            new()
            {
                Title = "The Matrix",
                Year = 1999,
                Duration = 136,
                Genres = new List<Genre> { action, scienceFiction },
                Actors = new List<Actor> { keanuReeves, laurenceFishburne },
                MovieDetails = new MovieDetails
                {
                    Synopsis = "A programmer questions reality and joins a struggle for human freedom.",
                    Language = "English",
                    Budget = 63_000_000m
                },
                Reviews = new List<Review>
                {
                    new()
                    {
                        ReviewerName = "Jamie",
                        Comment = "Inventive science fiction with striking action.",
                        Rating = 5
                    }
                }
            },
            new()
            {
                Title = "The Dark Knight",
                Year = 2008,
                Duration = 152,
                Genres = new List<Genre> { action, crime, drama },
                Actors = new List<Actor> { christianBale, heathLedger, morganFreeman },
                MovieDetails = new MovieDetails
                {
                    Synopsis = "A masked hero faces a criminal threat that challenges the city's trust.",
                    Language = "English",
                    Budget = 185_000_000m
                },
                Reviews = new List<Review>
                {
                    new()
                    {
                        ReviewerName = "Taylor",
                        Comment = "A tense crime drama supported by a strong ensemble.",
                        Rating = 5
                    },
                    new()
                    {
                        ReviewerName = "Jordan",
                        Comment = "Visually impressive and consistently engaging.",
                        Rating = 4
                    }
                }
            }
        };

        dbContext.Movies.AddRange(movies);
        dbContext.SaveChanges();
    }
}