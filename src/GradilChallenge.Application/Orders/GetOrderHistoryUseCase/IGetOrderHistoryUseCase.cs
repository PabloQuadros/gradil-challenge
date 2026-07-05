using GradilChallenge.Domain.Entities;

namespace GradilChallenge.Application.Orders.GetOrderHistoryUseCase;

public interface IGetOrderHistoryUseCase
{
    Task<IReadOnlyList<Order>> ExecuteAsync();
}
