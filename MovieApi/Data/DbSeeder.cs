using Microsoft.EntityFrameworkCore;
using MovieApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace MovieApi.Data
{
    public static class DbSeeder
    {
        public static void Seed(DbContext dbContext, bool _)
        {
            if (dbContext.Set<Movie>().Any())
            {
                return;
            }

            var movies = CreateMovies(
                GetOrAddDirector(dbContext, "Frank Darabont"),
                GetOrAddDirector(dbContext, "Lana Wachowski"),
                GetOrAddDirector(dbContext, "Christopher Nolan"),
                GetOrAddCountry(dbContext, "United States"));

            dbContext.Set<Movie>().AddRange(movies);
            dbContext.SaveChanges();
        }

        public static async Task SeedAsync(
            DbContext dbContext,
            bool _,
            CancellationToken cancellationToken)
        {
            if (await dbContext.Set<Movie>().AnyAsync(cancellationToken))
            {
                return;
            }

            var frankDarabont = await GetOrAddDirectorAsync(dbContext, "Frank Darabont", cancellationToken);
            var lanaWachowski = await GetOrAddDirectorAsync(dbContext, "Lana Wachowski", cancellationToken);
            var christopherNolan = await GetOrAddDirectorAsync(dbContext, "Christopher Nolan", cancellationToken);
            var unitedStates = await GetOrAddCountryAsync(dbContext, "United States", cancellationToken);
            var movies = CreateMovies(frankDarabont, lanaWachowski, christopherNolan, unitedStates);

            dbContext.Set<Movie>().AddRange(movies);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        private static List<Movie> CreateMovies(
            Director frankDarabont,
            Director lanaWachowski,
            Director christopherNolan,
            Country unitedStates)
        {
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
                Director = frankDarabont,
                Country = unitedStates,
                Genres = new List<Genre> { crime, drama },
                MovieActors = new List<MovieActor>
                {
                    new() { Actor = timRobbins, Role = "Andy Dufresne" },
                    new() { Actor = morganFreeman, Role = "Ellis Boyd \"Red\" Redding" }
                },
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
                Director = lanaWachowski,
                Country = unitedStates,
                Genres = new List<Genre> { action, scienceFiction },
                MovieActors = new List<MovieActor>
                {
                    new() { Actor = keanuReeves, Role = "Neo" },
                    new() { Actor = laurenceFishburne, Role = "Morpheus" }
                },
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
                Director = christopherNolan,
                Country = unitedStates,
                Genres = new List<Genre> { action, crime, drama },
                MovieActors = new List<MovieActor>
                {
                    new() { Actor = christianBale, Role = "Bruce Wayne / Batman" },
                    new() { Actor = heathLedger, Role = "Joker" },
                    new() { Actor = morganFreeman, Role = "Lucius Fox" }
                },
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

            return movies;
        }

        private static Director GetOrAddDirector(DbContext dbContext, string name)
        {
            var director = dbContext.Set<Director>().SingleOrDefault(item => item.Name == name);
            if (director is not null)
            {
                return director;
            }

            director = new Director { Name = name };
            dbContext.Set<Director>().Add(director);
            return director;
        }

        private static async Task<Director> GetOrAddDirectorAsync(
            DbContext dbContext,
            string name,
            CancellationToken cancellationToken)
        {
            var director = await dbContext.Set<Director>()
                .SingleOrDefaultAsync(item => item.Name == name, cancellationToken);
            if (director is not null)
            {
                return director;
            }

            director = new Director { Name = name };
            dbContext.Set<Director>().Add(director);
            return director;
        }

        private static Country GetOrAddCountry(DbContext dbContext, string name)
        {
            var country = dbContext.Set<Country>().SingleOrDefault(item => item.Name == name);
            if (country is not null)
            {
                return country;
            }

            country = new Country { Name = name };
            dbContext.Set<Country>().Add(country);
            return country;
        }

        private static async Task<Country> GetOrAddCountryAsync(
            DbContext dbContext,
            string name,
            CancellationToken cancellationToken)
        {
            var country = await dbContext.Set<Country>()
                .SingleOrDefaultAsync(item => item.Name == name, cancellationToken);
            if (country is not null)
            {
                return country;
            }

            country = new Country { Name = name };
            dbContext.Set<Country>().Add(country);
            return country;
        }
    }
}