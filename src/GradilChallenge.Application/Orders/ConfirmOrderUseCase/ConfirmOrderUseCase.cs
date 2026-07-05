using GradilChallenge.Application.Common;
using GradilChallenge.Domain.Entities;
using GradilChallenge.Domain.Repositories;

namespace GradilChallenge.Application.Orders.ConfirmOrderUseCase;

public sealed class ConfirmOrderUseCase(IOrderRepository orderRepository) : IConfirmOrderUseCase
{
    public async Task<Result<Order>> ExecuteAsync(Quote quote)
    {
        if (!Order.TryConfirm(quote, out var order, out var error))
            return Result<Order>.Failure(error.Message, error.Code);

        await orderRepository.AddAsync(order);
        return Result<Order>.Success(order);
    }
}
