using GradilChallenge.Application.Quotes.CalculateQuoteUseCase;
using GradilChallenge.Domain.Enums;

namespace GradilChallenge.Tests.Application;

public class CalculateQuoteUseCaseTests
{
    private readonly CalculateQuoteUseCase _useCase = new();

    [Fact]
    public async Task ExecuteAsync_ReturnsQuoteForValidInputs()
    {
        var result = await _useCase.ExecuteAsync(9, FenceHeight.Height153.Id, FenceColor.Green.Id);

        Assert.True(result.IsSuccess);
        Assert.Equal(4, result.Value.PanelCount);
        Assert.Equal(FenceHeight.Height153, result.Value.Height);
        Assert.Equal(FenceColor.Green, result.Value.Color);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public async Task ExecuteAsync_FailsForInvalidLength(double meters)
    {
        var result = await _useCase.ExecuteAsync(meters, FenceHeight.Height103.Id, FenceColor.NoPaint.Id);

        Assert.False(result.IsSuccess);
        Assert.Equal("length.invalid", result.ErrorCode);
        Assert.False(string.IsNullOrEmpty(result.ErrorMessage));
    }

    [Fact]
    public async Task ExecuteAsync_FailsForInvalidHeight()
    {
        var result = await _useCase.ExecuteAsync(10, heightId: 99, FenceColor.NoPaint.Id);

        Assert.False(result.IsSuccess);
        Assert.Equal("fenceheight.invalid", result.ErrorCode);
    }

    [Fact]
    public async Task ExecuteAsync_FailsForInvalidColor()
    {
        var result = await _useCase.ExecuteAsync(10, FenceHeight.Height103.Id, colorId: 99);

        Assert.False(result.IsSuccess);
        Assert.Equal("fencecolor.invalid", result.ErrorCode);
    }
}
