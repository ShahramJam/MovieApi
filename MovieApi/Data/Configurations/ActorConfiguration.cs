using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieApi.Models;

namespace MovieApi.Data.Configurations;

public sealed class ActorConfiguration : IEntityTypeConfiguration<Actor>
{
    public void Configure(EntityTypeBuilder<Actor> builder)
    {
        builder.HasKey(actor => actor.Id);
        builder.Property(actor => actor.Name).IsRequired();

        builder.HasMany(actor => actor.Movies)
            .WithMany(movie => movie.Actors)
            .UsingEntity<Dictionary<string, object>>(
                "MovieActor",
                movie => movie
                    .HasOne<Movie>()
                    .WithMany()
                    .HasForeignKey("MovieId")
                    .OnDelete(DeleteBehavior.Cascade),
                actor => actor
                    .HasOne<Actor>()
                    .WithMany()
                    .HasForeignKey("ActorId")
                    .OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.HasKey("MovieId", "ActorId");
                    join.ToTable("MovieActor");
                });
    }
}