using GradilChallenge.Infrastructure.DbContexts;
using GradilChallenge.Presentation.Services;
using GradilChallenge.Presentation.Views;
using GradilChallenge.Presentation.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GradilChallenge.Infrastructure.DependencyInjection;
using GradilChallenge.Application.DependencyInjection;
using System.Windows;

namespace GradilChallenge.Presentation;

public partial class App : System.Windows.Application
{
    private ServiceProvider? _serviceProvider;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            var services = new ServiceCollection();

            services.AddInfrastructure();
            services.AddApplication();
            services.AddSingleton<ShellViewModel>();
            services.AddSingleton<QuoteViewModel>();
            services.AddSingleton<OrderHistoryViewModel>();
            services.AddSingleton<FenceDrawingBuilder>();
            services.AddSingleton<ShellWindow>();

            _serviceProvider = services.BuildServiceProvider();

            await using (var scope = _serviceProvider.CreateAsyncScope())
            {
                var dbContextFactory = scope.ServiceProvider
                    .GetRequiredService<IDbContextFactory<GradilDbContext>>();

                await using var dbContext = await dbContextFactory.CreateDbContextAsync();
                await dbContext.Database.EnsureCreatedAsync();
            }

            var shellWindow = _serviceProvider.GetRequiredService<ShellWindow>();
            shellWindow.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Falha ao iniciar a aplicação:\n" + ex.Message,
                "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(-1);
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}

