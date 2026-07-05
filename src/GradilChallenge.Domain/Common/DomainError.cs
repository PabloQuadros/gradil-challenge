namespace GradilChallenge.Domain.Common;

public sealed record DomainError(string Code, string Message)
{
    public override string ToString() => Message;
}

