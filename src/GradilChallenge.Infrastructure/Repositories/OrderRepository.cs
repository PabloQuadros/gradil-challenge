using GradilChallenge.Domain.Entities;
using GradilChallenge.Domain.Repositories;
using GradilChallenge.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace GradilChallenge.Infrastructure.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly IDbContextFactory<GradilDbContext> _dbContextFactory;

    public OrderRepository(IDbContextFactory<GradilDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task AddAsync(Order order)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Order>> GetAllOrderedByMostRecentAsync()
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        return await dbContext.Orders
            .AsNoTracking()
            .OrderByDescending(order => order.ConfirmedAt)
            .ToListAsync();
    }
}
