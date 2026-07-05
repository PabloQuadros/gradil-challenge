
using GradilChallenge.Domain.Entities;
using GradilChallenge.Domain.Enums;
using GradilChallenge.Domain.ValueObjects;

namespace GradilChallenge.Application.Common.Quotes.CalculateQuoteUseCase;

public sealed class CalculateQuoteUseCase : ICalculateQuoteUseCase
{
    public Task<Result<Quote>> ExecuteAsync(double desiredLengthInMeters, int heightId, int colorId)
    {
        if (!Length.TryCreate(desiredLengthInMeters, out var desiredLength, out var lengthError))
            return Task.FromResult(Result<Quote>.Failure(lengthError.Message, lengthError.Code));

        if (!FenceHeight.TryFromId(heightId, out var height, out var heightError))
            return Task.FromResult(Result<Quote>.Failure(heightError.Message, heightError.Code));

        if (!FenceColor.TryFromId(colorId, out var color, out var colorError))
            return Task.FromResult(Result<Quote>.Failure(colorError.Message, colorError.Code));

        if (!Quote.TryCreate(desiredLength, height, color, panel: null, out var quote, out var quoteError))
            return Task.FromResult(Result<Quote>.Failure(quoteError.Message, quoteError.Code));

        return Task.FromResult(Result<Quote>.Success(quote));
    }
}
