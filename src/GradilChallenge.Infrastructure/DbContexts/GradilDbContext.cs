using GradilChallenge.Domain.Entities;
using GradilChallenge.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace GradilChallenge.Infrastructure.DbContexts;

public sealed class GradilDbContext : DbContext
{
    public DbSet<Order> Orders => Set<Order>();

    public GradilDbContext(DbContextOptions<GradilDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
    }
}
