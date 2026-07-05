using GradilChallenge.Domain.Repositories;
using GradilChallenge.Infrastructure.DbContexts;
using GradilChallenge.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GradilChallenge.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string? dbPath = null)
    {
        if (dbPath is null)
        {
            string dataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "GradilApp");
            Directory.CreateDirectory(dataFolder);

            dbPath = Path.Combine(dataFolder, "gradil.db");
        }

        services.AddDbContextFactory<GradilDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        services.AddScoped<IOrderRepository, OrderRepository>();

        return services;
    }
}
