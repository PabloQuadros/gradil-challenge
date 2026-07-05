using GradilChallenge.Domain.Common;
using GradilChallenge.Domain.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace GradilChallenge.Domain.Enums;

public sealed class FenceHeight : Enumeration
{
    public double HeightInMeters { get; }
    public int FastenersPerPost { get; }

    private FenceHeight(int id, string name, double heightInMeters, int fastenersPerPost)
        : base(id, name)
    {
        HeightInMeters = heightInMeters;
        FastenersPerPost = fastenersPerPost;
    }

    public static readonly FenceHeight Height103 = new(1, "1.03m", heightInMeters: 1.03, fastenersPerPost: 3);
    public static readonly FenceHeight Height153 = new(2, "1.53m", heightInMeters: 1.53, fastenersPerPost: 4);
    public static readonly FenceHeight Height203 = new(3, "2.03m", heightInMeters: 2.03, fastenersPerPost: 6);

    public static IEnumerable<FenceHeight> List() => GetAll<FenceHeight>();

    public static bool TryFromId(int id,
     [NotNullWhen(true)] out FenceHeight? height,
     [NotNullWhen(false)] out DomainError? error)
     => TryFromId(id, "altura de cerca", out height, out error);

    public static FenceHeight FromId(int id)
    {
        if (!TryFromId(id, out var height, out var error))
            throw new DomainException(error);

        return height;
    }

    public static FenceHeight FromName(string name) =>
        List().SingleOrDefault(h => h.Name == name)
        ?? throw new ArgumentException($"'{name}' não é uma altura de cerca válida.", nameof(name));
}
