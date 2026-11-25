using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;

namespace MovieApi.Data.Extenstions;

public static class SeedDataExtensions
{
    public static async Task SeedDataAsync(this WebApplication app, CancellationToken cancellationToken = default)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MovieDbContext>();

        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}