using GradilChallenge.Domain.Entities;
using GradilChallenge.Domain.Enums;
using GradilChallenge.Presentation.Models.Drawing;

namespace GradilChallenge.Presentation.Services;

public sealed class FenceDrawingBuilder
{
    public const double GroundY = 220;
    private const double PixelsPerMeter = 70;
    private const double PanelWidth = 130;
    private const double PostWidth = 12;
    private const double PostExtraHeight = 10;
    private const double VerticalWireSpacing = 12;
    private const double HorizontalWireSpacing = 25;
    private const int MaxPanelsToDrawFully = 4;

    public IReadOnlyList<DrawingShape> Build(Quote quote)
    {
        var shapes = new List<DrawingShape>();

        double panelHeightPx = quote.Height.HeightInMeters * PixelsPerMeter;
        double postHeightPx = panelHeightPx + PostExtraHeight;

        bool isRepresentative = quote.PanelCount > MaxPanelsToDrawFully;
        int panelsToDraw = isRepresentative ? MaxPanelsToDrawFully : quote.PanelCount;
        double suffixWidth = isRepresentative ? 60 : 0;

        string panelColor = quote.Color switch
        {
            var c when c == FenceColor.White => "#E8E8E8",
            var c when c == FenceColor.Black => "#2B2B2B",
            var c when c == FenceColor.Green => "#3C7A3C",
            _ => "#9AA0A6"
        };

        double x = 10;

        for (int i = 0; i < panelsToDraw; i++)
        {
            AddPost(shapes, x, postHeightPx);
            x += PostWidth;
            AddPanelMesh(shapes, x, panelHeightPx, panelColor);
            x += PanelWidth;
        }

        if(!quote.IsClosed || isRepresentative)
        {
            AddPost(shapes, x, postHeightPx);
            x += PostWidth;
        }

        if (isRepresentative)
        {
            shapes.Add(new TextShape
            {
                X = x + 12,
                Y = GroundY - (panelHeightPx / 2) - 14,
                Text = "...",
                FontSize = 28,
                Foreground = "#808080"
            });
        }

        shapes.Add(new TextShape
        {
            X = 10,
            Y = GroundY + 12,
            Text = $"{quote.SoldLength} — {quote.PanelCount} {(quote.PanelCount == 1 ? "painel" : "painéis")}, {quote.PostCount} postes",
            FontSize = 13,
            Foreground = "#404040"
        });

        return shapes;
    }

    private static void AddPost(List<DrawingShape> shapes, double x, double postHeightPx)
    {
        shapes.Add(new RectangleShape
        {
            X = x,
            Y = GroundY - postHeightPx,
            Width = PostWidth,
            Height = postHeightPx,
            Fill = "#4A4A4A",
            Stroke = "#2B2B2B"
        });

        shapes.Add(new RectangleShape
        {
            X = x - 2,
            Y = GroundY - postHeightPx - 4,
            Width = PostWidth + 4,
            Height = 4,
            Fill = "#2B2B2B",
            Stroke = "#2B2B2B"
        });
    }

    private static void AddPanelMesh(List<DrawingShape> shapes, double panelX, double panelHeightPx, string wireColor)
    {
        double panelTop = GroundY - panelHeightPx;

        for (double wx = panelX + VerticalWireSpacing; wx < panelX + PanelWidth; wx += VerticalWireSpacing)
        {
            shapes.Add(new LineShape
            {
                X = wx,
                Y = panelTop,
                X2 = wx,
                Y2 = GroundY,
                Stroke = wireColor,
                Thickness = 1.5
            });
        }

        for (double wy = panelTop + HorizontalWireSpacing; wy < GroundY; wy += HorizontalWireSpacing)
        {
            shapes.Add(new LineShape
            {
                X = panelX,
                Y = wy,
                X2 = panelX + PanelWidth,
                Y2 = wy,
                Stroke = wireColor,
                Thickness = 1.5
            });
        }

        shapes.Add(new LineShape
        {
            X = panelX,
            Y = panelTop,
            X2 = panelX + PanelWidth,
            Y2 = panelTop,
            Stroke = wireColor,
            Thickness = 2.5
        });
    }
}
