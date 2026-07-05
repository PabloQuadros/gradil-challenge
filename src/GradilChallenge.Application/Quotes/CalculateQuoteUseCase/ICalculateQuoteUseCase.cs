using GradilChallenge.Application.Common;
using GradilChallenge.Domain.Entities;

namespace GradilChallenge.Application.Quotes.CalculateQuoteUseCase;

public interface ICalculateQuoteUseCase
{
    Task<Result<Quote>> ExecuteAsync(double desiredLengthInMeters, int heightId, int colorId);
}
