namespace GradilChallenge.Presentation.Models.Drawing;

public sealed class LineShape : DrawingShape
{
    public double X2 { get; init; }
    public double Y2 { get; init; }
    public string Stroke { get; init; } = "#404040";
    public double Thickness { get; init; } = 1;
    public double LengthX => X2 - X;
    public double LengthY => Y2 - Y;
}
