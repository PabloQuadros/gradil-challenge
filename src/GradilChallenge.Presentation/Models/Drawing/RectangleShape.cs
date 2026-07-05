namespace GradilChallenge.Presentation.Models.Drawing;

public sealed class RectangleShape : DrawingShape
{
    public double Width { get; init; }
    public double Height { get; init; }
    public string Fill { get; init; } = "#808080";
    public string Stroke { get; init; } = "#404040";
}
