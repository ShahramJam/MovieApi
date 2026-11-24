using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieApi.Models;
using System.Collections.Generic;

namespace MovieApi.Data.Configurations;

public sealed class GenreConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.HasKey(genre => genre.Id);
        builder.Property(genre => genre.Name).IsRequired();

        builder.HasMany(genre => genre.Movies)
            .WithMany(movie => movie.Genres)
            .UsingEntity<Dictionary<string, object>>(
                "MovieGenre",
                movie => movie
                    .HasOne<Movie>()
                    .WithMany()
                    .HasForeignKey("MovieId")
                    .OnDelete(DeleteBehavior.Cascade),
                genre => genre
                    .HasOne<Genre>()
                    .WithMany()
                    .HasForeignKey("GenreId")
                    .OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.HasKey("MovieId", "GenreId");
                    join.ToTable("MovieGenre");
                });
    }
}