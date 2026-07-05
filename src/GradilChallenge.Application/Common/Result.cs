using System.Diagnostics.CodeAnalysis;

namespace GradilChallenge.Application.Common;

public sealed class Result<T>
{
    [MemberNotNullWhen(true, nameof(Value))]
    public bool IsSuccess { get; }

    public T? Value { get; }
    public string? ErrorMessage { get; }
    public string? ErrorCode { get; }


    private Result(bool isSuccess, T? value, string? errorMessage, string? errorCode)
    {
        IsSuccess = isSuccess;
        Value = value;
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
    }

    public static Result<T> Success(T value) => new(true, value, null, null);
    public static Result<T> Failure(string errorMessage, string errorCode) => new(false, default, errorMessage, errorCode);
}
