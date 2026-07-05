using GradilChallenge.Domain.Common;
using GradilChallenge.Domain.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace GradilChallenge.Domain.Enums;

public sealed class FenceColor : Enumeration
{
    private FenceColor(int id, string name) : base(id, name)
    {
    }

    public static readonly FenceColor NoPaint = new(1, "Sem pintura");
    public static readonly FenceColor White = new(2, "Branca");
    public static readonly FenceColor Black = new(3, "Preta");
    public static readonly FenceColor Green = new(4, "Verde");

    public static IEnumerable<FenceColor> List() => GetAll<FenceColor>();

    public static bool TryFromId(int id,
    [NotNullWhen(true)] out FenceColor? color,
    [NotNullWhen(false)] out DomainError? error)
    => TryFromId(id, "cor da cerca", out color, out error);

    public static FenceColor FromId(int id)
    {
        if (!TryFromId(id, out var color, out var error))
            throw new DomainException(error);

        return color;
    }

    public static FenceColor FromName(string name) =>
        List().SingleOrDefault(c => c.Name == name)
        ?? throw new ArgumentException($"'{name}' não é uma cor de cerca válida.", nameof(name));
}
