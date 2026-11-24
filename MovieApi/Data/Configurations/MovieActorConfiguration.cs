using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieApi.Models;

namespace MovieApi.Data.Configurations;

public sealed class MovieActorConfiguration : IEntityTypeConfiguration<MovieActor>
{
    public void Configure(EntityTypeBuilder<MovieActor> builder)
    {
        builder.ToTable("MovieActor");
        builder.HasKey(movieActor => new { movieActor.MovieId, movieActor.ActorId });
        builder.Property(movieActor => movieActor.Role)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(movieActor => movieActor.Movie)
            .WithMany(movie => movie.MovieActors)
            .HasForeignKey(movieActor => movieActor.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(movieActor => movieActor.Actor)
            .WithMany(actor => actor.MovieActors)
            .HasForeignKey(movieActor => movieActor.ActorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}