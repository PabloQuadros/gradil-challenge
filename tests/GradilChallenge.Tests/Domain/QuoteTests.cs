using GradilChallenge.Domain.Entities;
using GradilChallenge.Domain.Enums;
using GradilChallenge.Domain.ValueObjects;

namespace GradilChallenge.Tests.Domain;

public class QuoteTests
{
    [Theory]
    [InlineData(2.5, 1, 2.5)]
    [InlineData(9.0, 4, 10.0)]
    [InlineData(10.0, 4, 10.0)]
    [InlineData(0.5, 1, 2.5)]
    [InlineData(25.0, 10, 25.0)]
    public void TryCreate_CalculatesPanelsAndSoldLength(double desired, int expectedPanels, double expectedSold)
    {
        var quote = Quote.Create(Length.FromMeters(desired), FenceHeight.Height103, FenceColor.NoPaint);

        Assert.Equal(expectedPanels, quote.PanelCount);
        Assert.Equal(expectedSold, quote.SoldLength.Meters, precision: 2);
    }

    [Fact]
    public void TryCreate_CalculatesPostsFastenersAndScrews()
    {
        var quote = Quote.Create(Length.FromMeters(10), FenceHeight.Height153, FenceColor.White);

        Assert.Equal(5, quote.PostCount);
        Assert.Equal(20, quote.FastenerCount);
        Assert.Equal(20, quote.ScrewCount);
    }

    [Theory]
    [InlineData(1, 3)]
    [InlineData(2, 4)]
    [InlineData(3, 6)]
    public void TryCreate_FastenersVaryWithHeight(int heightId, int fastenersPerPost)
    {
        var height = FenceHeight.FromId(heightId);
        var quote = Quote.Create(Length.FromMeters(2.5), height, FenceColor.NoPaint);

        Assert.Equal(quote.PostCount * fastenersPerPost, quote.FastenerCount);
    }

    [Fact]
    public void TryCreate_UsesStandardPanelWhenNotProvided()
    {
        var quote = Quote.Create(Length.FromMeters(5), FenceHeight.Height103, FenceColor.NoPaint);

        Assert.Equal(FencePanel.Standard, quote.Panel);
    }

    [Fact]
    public void HasDifference_TrueWhenHasLeftoverLength()
    {
        var quote = Quote.Create(Length.FromMeters(9), FenceHeight.Height103, FenceColor.NoPaint);

        Assert.Equal(1.0, quote.DifferenceInMeters, precision: 2);
        Assert.True(quote.HasDifference);
    }

    [Fact]
    public void HasDifference_FalseWhenExactMultiple()
    {
        var quote = Quote.Create(Length.FromMeters(10), FenceHeight.Height103, FenceColor.NoPaint);

        Assert.Equal(0.0, quote.DifferenceInMeters, precision: 2);
        Assert.False(quote.HasDifference);
    }

    [Fact]
    public void TryCreate_FailsWithoutLength()
    {
        var ok = Quote.TryCreate(null, FenceHeight.Height103, FenceColor.NoPaint, null, out var quote, out var error);

        Assert.False(ok);
        Assert.Null(quote);
        Assert.Equal("quote.length.required", error!.Code);
    }

    [Fact]
    public void TryCreate_FailsWithoutHeight()
    {
        var ok = Quote.TryCreate(Length.FromMeters(5), null, FenceColor.NoPaint, null, out _, out var error);

        Assert.False(ok);
        Assert.Equal("quote.height.required", error!.Code);
    }

    [Fact]
    public void TryCreate_FailsWithoutColor()
    {
        var ok = Quote.TryCreate(Length.FromMeters(5), FenceHeight.Height103, null, null, out _, out var error);

        Assert.False(ok);
        Assert.Equal("quote.color.required", error!.Code);
    }
}
