using GradilChallenge.Domain.Enums;

namespace GradilChallenge.Tests.Domain;

public class EnumerationTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void TryFromId_finds_valid_ids(int id)
    {
        var ok = FenceHeight.TryFromId(id, out var height, out var error);

        Assert.True(ok);
        Assert.NotNull(height);
        Assert.Null(error);
        Assert.Same(FenceHeight.FromId(id), height);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(99)]
    [InlineData(-1)]
    public void TryFromId_fails_for_invalid_ids(int id)
    {
        var ok = FenceHeight.TryFromId(id, out var height, out var error);

        Assert.False(ok);
        Assert.Null(height);
        Assert.Equal("fenceheight.invalid", error!.Code);
    }

    [Fact]
    public void Equality_requires_same_type_and_id()
    {
        Assert.True(FenceHeight.Height103 == FenceHeight.FromId(1));
        Assert.False(FenceHeight.Height103 != FenceHeight.FromId(1));
        Assert.False(FenceHeight.Height103.Equals(FenceColor.NoPaint));
    }

    [Fact]
    public void Null_comparison_is_safe()
    {
        Assert.False(FenceHeight.Height103 == null);
        Assert.True(FenceHeight.Height103 != null);
        Assert.False(FenceHeight.Height103.Equals(null));
    }
}
