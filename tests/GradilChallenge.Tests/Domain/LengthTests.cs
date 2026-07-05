using GradilChallenge.Domain.Exceptions;
using GradilChallenge.Domain.ValueObjects;

namespace GradilChallenge.Tests.Domain;

public class LengthTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-0.01)]
    public void TryCreate_rejects_non_positive_values(double meters)
    {
        var ok = Length.TryCreate(meters, out var length, out var error);

        Assert.False(ok);
        Assert.Null(length);
        Assert.Equal("length.invalid", error!.Code);
    }

    [Fact]
    public void FromMeters_throws_for_invalid_value()
    {
        var ex = Assert.Throws<DomainException>(() => Length.FromMeters(0));
        Assert.Equal("length.invalid", ex.Error.Code);
    }

    [Fact]
    public void Equality_normalizes_floating_point_noise()
        => Assert.Equal(Length.FromMeters(0.1 + 0.2), Length.FromMeters(0.3));

    [Fact]
    public void Equal_lengths_have_equal_hash_codes()
    {
        var a = Length.FromMeters(0.1 + 0.2);
        var b = Length.FromMeters(0.3);

        Assert.Equal(a, b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Operators_compare_by_meters()
    {
        var small = Length.FromMeters(1);
        var big = Length.FromMeters(2);

        Assert.True(big > small);
        Assert.True(small < big);
        Assert.True(big >= small);
        Assert.True(small <= big);
        Assert.Equal(3.0, (small + big).Meters, precision: 4);
        Assert.Equal(1.0, (big - small).Meters, precision: 4);
    }
}
