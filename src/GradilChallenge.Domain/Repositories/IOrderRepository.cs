using GradilChallenge.Domain.Entities;

namespace GradilChallenge.Domain.Repositories;

public interface IOrderRepository
{
    Task AddAsync(Order order);
    Task<IReadOnlyList<Order>> GetAllOrderedByMostRecentAsync();
}
