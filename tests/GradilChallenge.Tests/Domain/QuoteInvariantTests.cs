using GradilChallenge.Domain.Entities;
using GradilChallenge.Domain.Enums;
using GradilChallenge.Domain.ValueObjects;

namespace GradilChallenge.Tests.Domain;

public class QuoteInvariantTests
{
    public static TheoryData<double> Lengths()
    {
        var data = new TheoryData<double>();

        for (double m = 0.1; m <= 100; m += 0.37)
            data.Add(Math.Round(m, 2));

        // Edge cases
        data.Add(0.01);
        data.Add(2.5);
        data.Add(2.51);
        data.Add(1000);

        return data;
    }

    [Theory]
    [MemberData(nameof(Lengths))]
    public void Invariants_hold_for_any_length(double meters)
    {
        foreach (var height in FenceHeight.List())
        {
            var quote = Quote.Create(Length.FromMeters(meters), height, FenceColor.NoPaint);
            double panel = quote.Panel.LengthInMeters;

            // Sold length always covers the desired length…
            Assert.True(quote.SoldLength.Meters >= meters - 1e-9,
                $"SoldLength {quote.SoldLength.Meters} < desired {meters}");

            // Without wasting a full extra panel
            Assert.True(quote.SoldLength.Meters - meters < panel,
                $"Leftover {quote.SoldLength.Meters - meters} >= panel {panel} for {meters}m");

            // Sold length is an exact multiple of panels
            Assert.Equal(quote.PanelCount * panel, quote.SoldLength.Meters, precision: 6);

            // Structure: always one more post than panels
            Assert.True(quote.PanelCount >= 1);
            Assert.Equal(quote.PanelCount + 1, quote.PostCount);

            // Fasteners and screws derive from posts
            Assert.Equal(quote.PostCount * height.FastenersPerPost, quote.FastenerCount);
            Assert.Equal(quote.PostCount * 4, quote.ScrewCount);

            // Difference is never negative and agrees with HasDifference
            Assert.True(quote.DifferenceInMeters >= 0);
            Assert.Equal(quote.DifferenceInMeters >= 0.01, quote.HasDifference);
        }
    }
}
