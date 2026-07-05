using GradilChallenge.Domain.Entities;
using GradilChallenge.Domain.Enums;
using GradilChallenge.Domain.ValueObjects;
using GradilChallenge.Infrastructure.DbContexts;
using GradilChallenge.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GradilChallenge.Tests.Infrastructure;

public sealed class OrderRepositoryTests : IDisposable
{
    private readonly string _dbPath;
    private readonly OrderRepository _repository;

    private sealed class TestDbContextFactory(DbContextOptions<GradilDbContext> options)
        : IDbContextFactory<GradilDbContext>
    {
        public GradilDbContext CreateDbContext() => new(options);
    }

    public OrderRepositoryTests()
    {
        _dbPath = Path.Combine(Path.GetTempPath(), $"gradil-tests-{Guid.NewGuid():N}.db");

        var options = new DbContextOptionsBuilder<GradilDbContext>()
            .UseSqlite($"Data Source={_dbPath};Pooling=False")
        .Options;

        using (var context = new GradilDbContext(options))
            context.Database.EnsureCreated();

        _repository = new OrderRepository(new TestDbContextFactory(options));
    }

    public void Dispose()
    {
        if (File.Exists(_dbPath))
            File.Delete(_dbPath);
    }

    private static Order CreateOrder(double meters, FenceHeight height, FenceColor color) =>
        Order.Confirm(Quote.Create(Length.FromMeters(meters), height, color));

    [Fact]
    public async Task AddAsync_PersistsAndRetrievesCompleteOrder()
    {
        var order = CreateOrder(9, FenceHeight.Height153, FenceColor.Green);

        await _repository.AddAsync(order);
        var stored = (await _repository.GetAllOrderedByMostRecentAsync()).Single();

        Assert.Equal(order.Id, stored.Id);
        Assert.Equal(9, stored.Quote.DesiredLength.Meters, precision: 2);
        Assert.Equal(10, stored.Quote.SoldLength.Meters, precision: 2);
        Assert.Equal(FenceHeight.Height153, stored.Quote.Height);
        Assert.Equal(FenceColor.Green, stored.Quote.Color);
        Assert.Equal(FencePanel.Standard, stored.Quote.Panel);
        Assert.Equal(4, stored.Quote.PanelCount);
        Assert.Equal(5, stored.Quote.PostCount);
        Assert.Equal(20, stored.Quote.FastenerCount);
        Assert.Equal(20, stored.Quote.ScrewCount);
    }

    [Fact]
    public async Task GetAllOrderedByMostRecentAsync_OrdersByMostRecent()
    {
        await _repository.AddAsync(CreateOrder(2.5, FenceHeight.Height103, FenceColor.NoPaint));
        await _repository.AddAsync(CreateOrder(5, FenceHeight.Height153, FenceColor.White));
        await _repository.AddAsync(CreateOrder(10, FenceHeight.Height203, FenceColor.Black));

        var orders = await _repository.GetAllOrderedByMostRecentAsync();

        Assert.Equal(3, orders.Count);
        for (int i = 1; i < orders.Count; i++)
            Assert.True(orders[i - 1].ConfirmedAt >= orders[i].ConfirmedAt,
                "Orders should be ordered from most recent to oldest.");
    }

    [Fact]
    public async Task GetAllOrderedByMostRecentAsync_ReturnsEmptyWhenNoOrders()
    {
        var orders = await _repository.GetAllOrderedByMostRecentAsync();

        Assert.Empty(orders);
    }
}
