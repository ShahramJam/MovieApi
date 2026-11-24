using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieApi.Models;

namespace MovieApi.Data.Configurations;

public sealed class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.HasKey(movie => movie.Id);
        builder.Property(movie => movie.Title).IsRequired();
        builder.Property(movie => movie.Year).IsRequired();
        builder.Property(movie => movie.Duration).IsRequired();

        builder.HasOne(movie => movie.Director)
            .WithMany(director => director.Movies)
            .HasForeignKey(movie => movie.DirectorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(movie => movie.Country)
            .WithMany(country => country.Movies)
            .HasForeignKey(movie => movie.CountryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}