using MovieApi.Repositories.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MovieApi.Data
{
    public sealed class UnitOfWork(
        MovieDbContext context,
        Lazy<IMovieRepository> movies,
        Lazy<IActorRepository> actors,
        Lazy<IReviewRepository> reviews,
        Lazy<IGenreRepository> genres,
        Lazy<IDirectorRepository> directors,
        Lazy<ICountryRepository> countries,
        Lazy<IMovieActorRepository> movieActors,
        Lazy<IReportRepository> reports) : IUnitOfWork
    {
        public IMovieRepository Movies => movies.Value;
        public IActorRepository Actors => actors.Value;
        public IReviewRepository Reviews => reviews.Value;
        public IGenreRepository Genres => genres.Value;
        public IDirectorRepository Directors => directors.Value;
        public ICountryRepository Countries => countries.Value;
        public IMovieActorRepository MovieActors => movieActors.Value;
        public IReportRepository Reports => reports.Value;

        public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
            context.SaveChangesAsync(ct);
    }
}