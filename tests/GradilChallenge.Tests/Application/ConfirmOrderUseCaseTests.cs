using GradilChallenge.Application.Orders.ConfirmOrderUseCase;
using GradilChallenge.Domain.Entities;
using GradilChallenge.Domain.Enums;
using GradilChallenge.Domain.Repositories;
using GradilChallenge.Domain.ValueObjects;

namespace GradilChallenge.Tests.Application;

public class ConfirmOrderUseCaseTests
{
    private sealed class FakeOrderRepository : IOrderRepository
    {
        public List<Order> Added { get; } = new();

        public Task AddAsync(Order order)
        {
            Added.Add(order);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<Order>> GetAllOrderedByMostRecentAsync() =>
            Task.FromResult<IReadOnlyList<Order>>(
                Added.OrderByDescending(o => o.ConfirmedAt).ToList());
    }

    private static Quote CreateQuote() =>
        Quote.Create(Length.FromMeters(5), FenceHeight.Height203, FenceColor.Black);

    [Fact]
    public async Task ExecuteAsync_ConfirmsAndPersistsOrder()
    {
        var repository = new FakeOrderRepository();
        var useCase = new ConfirmOrderUseCase(repository);
        var quote = CreateQuote();

        var result = await useCase.ExecuteAsync(quote);

        Assert.True(result.IsSuccess);
        Assert.Single(repository.Added);
        Assert.Same(quote, repository.Added[0].Quote);
    }

    [Fact]
    public async Task ExecuteAsync_FailsWithoutQuoteAndDoesNotPersist()
    {
        var repository = new FakeOrderRepository();
        var useCase = new ConfirmOrderUseCase(repository);

        var result = await useCase.ExecuteAsync(null!);

        Assert.False(result.IsSuccess);
        Assert.Equal("order.quote.required", result.ErrorCode);
        Assert.Empty(repository.Added);
    }
}