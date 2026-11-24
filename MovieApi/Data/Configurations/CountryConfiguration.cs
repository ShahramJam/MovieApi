using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieApi.Models;

namespace MovieApi.Data.Configurations;

public sealed class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.HasKey(country => country.Id);
        builder.Property(country => country.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.HasIndex(country => country.Name).IsUnique();
    }
}