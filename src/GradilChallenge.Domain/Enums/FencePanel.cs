using GradilChallenge.Domain.Common;

namespace GradilChallenge.Domain.Enums;

public sealed class FencePanel : Enumeration
{

    public double LengthInMeters { get; }

    private FencePanel(int id, string name, double lengthInMeters) : base(id, name)
    {
        LengthInMeters = lengthInMeters;
    }

    public static readonly FencePanel Standard = new(1, "Painel padrão", lengthInMeters: 2.5);

    public static IEnumerable<FencePanel> List() => GetAll<FencePanel>();

    public static FencePanel FromId(int id) =>
        List().SingleOrDefault(p => p.Id == id)
        ?? throw new ArgumentException($"'{id}' não é um painel válido.", nameof(id));
}
