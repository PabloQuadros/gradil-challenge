using GradilChallenge.Domain.Common;
using GradilChallenge.Domain.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace GradilChallenge.Domain.ValueObjects;

public sealed class Length : IEquatable<Length>, IComparable<Length>
{
    public double Meters { get; }

    private Length(double meters) => Meters = meters;

    public static bool TryCreate(double meters,
        [NotNullWhen(true)] out Length? length,
        [NotNullWhen(false)] out DomainError? error)
    {
        length = null;
        error = null;

        if (meters <= 0)
        {
            error = new DomainError("length.invalid", "O comprimento deve ser maior que zero.");
            return false;
        }

        length = new Length(meters);
        return true;
    }

    public static Length FromMeters(double meters)
    {
        if (!TryCreate(meters, out var length, out var error))
            throw new DomainException(error);

        return length;
    }

    public static Length operator +(Length a, Length b) => FromMeters(a.Meters + b.Meters);
    public static Length operator -(Length a, Length b) => FromMeters(a.Meters - b.Meters);
    public static bool operator >(Length a, Length b) => a.Meters > b.Meters;
    public static bool operator <(Length a, Length b) => a.Meters < b.Meters;
    public static bool operator >=(Length a, Length b) => a.Meters >= b.Meters;
    public static bool operator <=(Length a, Length b) => a.Meters <= b.Meters;

    public override string ToString() => $"{Meters:0.00} m";

    public bool Equals(Length? other)
    {
        if (other is null) return false;
        return Math.Abs(Meters - other.Meters) < 0.0001;
    }

    public override bool Equals(object? obj) => Equals(obj as Length);
    public override int GetHashCode() => Math.Round(Meters, 4).GetHashCode();
    public int CompareTo(Length? other) => Meters.CompareTo(other?.Meters ?? double.MinValue);
}
