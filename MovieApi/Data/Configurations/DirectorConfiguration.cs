using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieApi.Models;

namespace MovieApi.Data.Configurations;

public sealed class DirectorConfiguration : IEntityTypeConfiguration<Director>
{
    public void Configure(EntityTypeBuilder<Director> builder)
    {
        builder.HasKey(director => director.Id);
        builder.Property(director => director.Name)
            .IsRequired()
            .HasMaxLength(150);
        builder.HasIndex(director => director.Name).IsUnique();
    }
}