namespace GradilChallenge.Presentation.Models.Drawing;

public sealed class TextShape : DrawingShape
{
    public string Text { get; init; } = "";
    public double FontSize { get; init; } = 12;
    public string Foreground { get; init; } = "#404040";
}
