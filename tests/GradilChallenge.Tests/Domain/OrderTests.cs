using GradilChallenge.Domain.Entities;
using GradilChallenge.Domain.Enums;
using GradilChallenge.Domain.ValueObjects;

namespace GradilChallenge.Tests.Domain;

public class OrderTests
{
    private static Quote CreateQuote() =>
        Quote.Create(Length.FromMeters(10), FenceHeight.Height103, FenceColor.NoPaint);

    [Fact]
    public void TryConfirm_fails_without_a_quote()
    {
        var ok = Order.TryConfirm(null, out var order, out var error);

        Assert.False(ok);
        Assert.Null(order);
        Assert.Equal("order.quote.required", error!.Code);
    }

    [Fact]
    public void TryConfirm_succeeds_with_a_valid_quote()
    {
        var quote = CreateQuote();

        var ok = Order.TryConfirm(quote, out var order, out var error);

        Assert.True(ok);
        Assert.Null(error);
        Assert.NotNull(order);
        Assert.Same(quote, order.Quote);
    }

    [Fact]
    public void Confirm_generates_distinct_ids_per_order()
    {
        var quote = CreateQuote();

        var first = Order.Confirm(quote);
        var second = Order.Confirm(quote);

        Assert.NotEqual(first.Id, second.Id);
    }
}
