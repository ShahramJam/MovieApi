using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieApi.Models;

namespace MovieApi.Data.Configurations;

public sealed class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasKey(review => review.Id);
        builder.ToTable(
            "Reviews",
            table => table.HasCheckConstraint(
                "CK_Reviews_Rating_Range",
                "Rating >= 1 AND Rating <= 5"));

        builder.HasOne(review => review.Movie)
            .WithMany(movie => movie.Reviews)
            .HasForeignKey(review => review.MovieId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}