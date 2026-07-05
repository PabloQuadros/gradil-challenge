using GradilChallenge.Domain.Common;
using GradilChallenge.Domain.Enums;
using GradilChallenge.Domain.Exceptions;
using GradilChallenge.Domain.ValueObjects;
using System.Diagnostics.CodeAnalysis;

namespace GradilChallenge.Domain.Entities;

public sealed class Quote
{
    public Length DesiredLength { get; }
    public FenceHeight Height { get; }
    public Length SoldLength { get; }
    public FenceColor Color { get; }
    public FencePanel Panel { get; }
    public int PanelCount { get; }
    public int PostCount { get; }
    public int FastenerCount { get; }
    public int ScrewCount { get; }
    public double DifferenceInMeters => Math.Round(SoldLength.Meters - DesiredLength.Meters, 2);
    public bool HasDifference => DifferenceInMeters > 0;

    private Quote(
        Length desiredLength,
        FenceHeight height,
        FenceColor color,
        Length soldLength,
        FencePanel panel,
        int panelCount,
        int postCount,
        int fastenerCount,
        int screwCount)
    {
        DesiredLength = desiredLength;
        Height = height;
        Color = color;
        SoldLength = soldLength;
        Panel = panel;
        PanelCount = panelCount;
        PostCount = postCount;
        FastenerCount = fastenerCount;
        ScrewCount = screwCount;
    }

    public static bool TryCreate(Length? desiredLength, FenceHeight? height, FenceColor? color, FencePanel? panel,
    [NotNullWhen(true)] out Quote? quote,
    [NotNullWhen(false)] out DomainError? error)
    {
        quote = null;
        error = null;

        if (desiredLength is null)
        {
            error = new DomainError("quote.length.required", "O comprimento é obrigatório.");
            return false;
        }
        if (height is null)
        {
            error = new DomainError("quote.height.required", "A altura da cerca é obrigatória.");
            return false;
        }
        if (color is null)
        {
            error = new DomainError("quote.color.required", "A cor da cerca é obrigatória.");
            return false;
        }

        var resolvedPanel = panel ?? FencePanel.Standard;

        int panelCount = (int)Math.Ceiling(desiredLength.Meters / resolvedPanel.LengthInMeters);
        var soldLength = Length.FromMeters(panelCount * resolvedPanel.LengthInMeters);
        int postCount = panelCount + 1;
        int fastenersCount = postCount * height.FastenersPerPost;
        int screwCount = (panelCount + 1) * 4;

        quote = new Quote(desiredLength, height, color, soldLength, resolvedPanel,
                          panelCount, postCount, fastenersCount, screwCount);
        return true;
    }

    public static Quote Create(Length desiredLength, FenceHeight height, FenceColor color, FencePanel? panel = null)
    {
        if (!TryCreate(desiredLength, height, color, panel, out var quote, out var error))
            throw new DomainException(error);

        return quote;
    }
}
