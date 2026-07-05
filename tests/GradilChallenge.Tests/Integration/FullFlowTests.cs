using GradilChallenge.Application.Orders.ConfirmOrderUseCase;
using GradilChallenge.Application.Orders.GetOrderHistoryUseCase;
using GradilChallenge.Application.Quotes.CalculateQuoteUseCase;
using GradilChallenge.Application.DependencyInjection;
using GradilChallenge.Domain.Enums;
using GradilChallenge.Infrastructure.DbContexts;
using GradilChallenge.Infrastructure.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GradilChallenge.Tests.Integration;

public sealed class FullFlowTests : IAsyncLifetime, IDisposable
{
    private readonly string _dbPath =
        Path.Combine(Path.GetTempPath(), $"gradil-e2e-{Guid.NewGuid():N}.db;Pooling=False");

    private ServiceProvider _provider = null!;

    public async Task InitializeAsync()
    {
        var services = new ServiceCollection();
        services.AddInfrastructure(_dbPath);
        services.AddApplication();
        _provider = services.BuildServiceProvider();

        var factory = _provider.GetRequiredService<IDbContextFactory<GradilDbContext>>();
        await using var context = await factory.CreateDbContextAsync();
        await context.Database.EnsureCreatedAsync();
    }

    Task IAsyncLifetime.DisposeAsync()
    {
        Dispose();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _provider?.Dispose();
        if (File.Exists(_dbPath))
            File.Delete(_dbPath);
    }

    [Fact]
    public void Container_ResolvesAllUseCases()
    {
        using var scope = _provider.CreateScope();

        Assert.NotNull(scope.ServiceProvider.GetRequiredService<ICalculateQuoteUseCase>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IConfirmOrderUseCase>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IGetOrderHistoryUseCase>());
    }

    [Fact]
    public async Task FullFlow_CalculateConfirmAndListHistory()
    {
        using var scope = _provider.CreateScope();
        var calculate = scope.ServiceProvider.GetRequiredService<ICalculateQuoteUseCase>();
        var confirm = scope.ServiceProvider.GetRequiredService<IConfirmOrderUseCase>();
        var history = scope.ServiceProvider.GetRequiredService<IGetOrderHistoryUseCase>();

        // 1. Calculate the quote (12m, 1.53m, green)
        var quoteResult = await calculate.ExecuteAsync(12, FenceHeight.Height153.Id, FenceColor.Green.Id);
        Assert.True(quoteResult.IsSuccess);
        Assert.Equal(5, quoteResult.Value.PanelCount); // ceil(12/2.5)
        Assert.Equal(12.5, quoteResult.Value.SoldLength.Meters, precision: 2);

        // 2. Confirm the order
        var orderResult = await confirm.ExecuteAsync(quoteResult.Value);
        Assert.True(orderResult.IsSuccess);

        // 3. History retrieves the order persisted in SQLite, with the intact quote
        var orders = await history.ExecuteAsync();
        var stored = Assert.Single(orders);
        Assert.Equal(orderResult.Value.Id, stored.Id);
        Assert.Equal(5, stored.Quote.PanelCount);
        Assert.Equal(FenceColor.Green, stored.Quote.Color);
    }

    [Fact]
    public async Task FullFlow_InvalidInputDoesNotReachDatabase()
    {
        using var scope = _provider.CreateScope();
        var calculate = scope.ServiceProvider.GetRequiredService<ICalculateQuoteUseCase>();
        var history = scope.ServiceProvider.GetRequiredService<IGetOrderHistoryUseCase>();

        var result = await calculate.ExecuteAsync(-3, FenceHeight.Height103.Id, FenceColor.NoPaint.Id);

        Assert.False(result.IsSuccess);
        Assert.Empty(await history.ExecuteAsync());
    }
}
