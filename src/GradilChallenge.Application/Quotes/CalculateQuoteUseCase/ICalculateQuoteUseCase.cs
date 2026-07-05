using GradilChallenge.Domain.Entities;

namespace GradilChallenge.Application.Common.Quotes.CalculateQuoteUseCase;

public interface ICalculateQuoteUseCase
{
    Task<Result<Quote>> ExecuteAsync(double desiredLengthInMeters, int heightId, int colorId);
}
