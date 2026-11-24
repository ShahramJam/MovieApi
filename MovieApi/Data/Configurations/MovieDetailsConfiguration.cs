using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieApi.Models;

namespace MovieApi.Data.Configurations;

public sealed class MovieDetailsConfiguration : IEntityTypeConfiguration<MovieDetails>
{
    public void Configure(EntityTypeBuilder<MovieDetails> builder)
    {
        builder.HasKey(details => details.Id);
        builder.Property(details => details.Budget)
            .HasPrecision(18, 2);

        builder.HasOne(details => details.Movie)
            .WithOne(movie => movie.MovieDetails)
            .HasForeignKey<MovieDetails>(details => details.MovieId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}