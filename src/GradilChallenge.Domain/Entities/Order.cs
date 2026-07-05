using GradilChallenge.Domain.Common;
using GradilChallenge.Domain.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace GradilChallenge.Domain.Entities;

public sealed class Order
{
    public Guid Id { get; }
    public Quote Quote { get; }
    public DateTime ConfirmedAt { get; }
    public DateTime ConfirmedAtLocal => ConfirmedAt.ToLocalTime();


    private Order(Guid id, Quote quote, DateTime confirmedAt)
    {
        Id = id;
        Quote = quote;
        ConfirmedAt = confirmedAt;
    }

    private Order()
    {
    }

    public static bool TryConfirm(Quote? quote,
    [NotNullWhen(true)] out Order? order,
    [NotNullWhen(false)] out DomainError? error)
    {
        order = null;
        error = null;

        if (quote is null)
        {
            error = new DomainError("order.quote.required", "Um pedido precisa de uma cotação.");
            return false;
        }

        order = new Order(Guid.NewGuid(), quote, DateTime.UtcNow);
        return true;
    }

    public static Order Confirm(Quote quote)
    {
        if (!TryConfirm(quote, out var order, out var error))
            throw new DomainException(error);

        return order;
    }
}
