using GradilChallenge.Application.Common;
using GradilChallenge.Domain.Entities;

namespace GradilChallenge.Application.Orders.ConfirmOrderUseCase;

public interface IConfirmOrderUseCase
{
    Task<Result<Order>> ExecuteAsync(Quote quote);
}
