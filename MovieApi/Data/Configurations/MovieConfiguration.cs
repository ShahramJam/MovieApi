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
    }
}