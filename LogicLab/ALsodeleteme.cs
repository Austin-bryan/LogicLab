using LogicLab;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

public class DeleteMe2 : FrameworkElement
{
    private static List<Point> vertices = [];

    // Add Bezier segments to the batch
    public static void AddBezierSegment(Point start, Point controlPoint1, Point controlPoint2, Point end)
    {
        vertices.Add(start);
        vertices.Add(controlPoint1);
        vertices.Add(controlPoint2);
        vertices.Add(end);
    }

    // Perform batch rendering
    protected override void OnRender(DrawingContext dc)
    {
        base.OnRender(dc);

        StreamGeometry geometry = new();
        using (StreamGeometryContext ctx = geometry.Open())
        {
            ctx.BeginFigure(vertices[0], false, false);
            for (int i = 1; i + 2 < vertices.Count; i += 3)
                ctx.BezierTo(vertices[i], vertices[i + 1], vertices[i + 2], true, false);
        }

        // Render the batched Bezier segments
        dc.DrawGeometry(null, new Pen(Brushes.Black, 2), geometry);

        vertices.Clear();
    }
}