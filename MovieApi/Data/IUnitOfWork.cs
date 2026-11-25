using MovieApi.Repositories.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace MovieApi.Data
{
    public interface IUnitOfWork
    {
        IMovieRepository Movies { get; }
        IActorRepository Actors { get; }
        IReviewRepository Reviews { get; }
        IGenreRepository Genres { get; }
        IDirectorRepository Directors { get; }
        ICountryRepository Countries { get; }
        IMovieActorRepository MovieActors { get; }
        IReportRepository Reports { get; }

        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}