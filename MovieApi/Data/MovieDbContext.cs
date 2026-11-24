using Microsoft.EntityFrameworkCore;
using MovieApi.Models;

namespace MovieApi.Data;

public class MovieDbContext : DbContext
{
    public MovieDbContext(DbContextOptions<MovieDbContext> options)
        : base(options)
    {
    }

    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<MovieDetails> MovieDetails => Set<MovieDetails>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Actor> Actors => Set<Actor>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<MovieActor> MovieActors => Set<MovieActor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MovieDbContext).Assembly);
    }
}