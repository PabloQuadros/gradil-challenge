using GradilChallenge.Domain.Entities;
using GradilChallenge.Domain.Repositories;

namespace GradilChallenge.Application.Orders.GetOrderHistoryUseCase;

public sealed class GetOrderHistoryUseCase(IOrderRepository orderRepository) : IGetOrderHistoryUseCase
{
    public async Task<IReadOnlyList<Order>> ExecuteAsync()
        => await orderRepository.GetAllOrderedByMostRecentAsync();
}
