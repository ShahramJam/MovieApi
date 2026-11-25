using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MovieApi.Data;
using System;
using System.Linq;
using System.Reflection;

namespace MovieApi.Extensions;

public static class PersistenceExtensions
{
    public static IServiceCollection AddSqlServerPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MovieDb")
            ?? throw new InvalidOperationException("Connection string 'MovieDb' not found.");

        services.AddDbContext<MovieDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddRepositoriesFromAssembly(Assembly.GetExecutingAssembly());
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    private static IServiceCollection AddRepositoriesFromAssembly(
       this IServiceCollection services,
       Assembly? assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();

        // Enable Lazy<T> support for UnitOfWork
        services.AddTransient(typeof(Lazy<>), typeof(LazyService<>));

        var repositoryTypes = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false }
                     && t.Name.EndsWith("Repository", StringComparison.Ordinal));

        foreach (var implementationType in repositoryTypes)
        {
            // Finds IMovieRepository for MovieRepository, IActorRepository for ActorRepository, etc.
            var interfaceType = implementationType.GetInterfaces()
                .FirstOrDefault(i => i.Name == $"I{implementationType.Name}");

            if (interfaceType is not null)
            {
                services.AddScoped(interfaceType, implementationType);
            }
        }

        return services;
    }

    private sealed class LazyService<T>(IServiceProvider provider)
        : Lazy<T>(provider.GetRequiredService<T>) where T : notnull;
}
