using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LogicLab;

public partial class Wire : LogicComponent
{
    private readonly Path splinePath = new()
    {
        Stroke = new SolidColorBrush(Color.FromRgb(200, 200, 200)),
        StrokeThickness = 4
    };
    public Wire(MainWindow mainWindow, Point startPoint)
    {
        Dragger = null;
        mainWindow.MainGrid.Children.Insert(0, splinePath);

        this.startPoint       = startPoint;
        splinePath.MouseDown += Spline_MouseDown;
        splinePath.MouseUp   += Gate_MouseUp;
        splinePath.Effect     = Effect;
    }
    private Point startPoint;

    public void SetEndPoint(EPortType portType, Point end)
    {
        Draw(portType, end);
    }

    public void Draw(EPortType portType Point end)
    {
        const int offset = 25;

        Point start = new(startPoint.X, startPoint.Y - offset);
        end = new Point(end.X, end.Y - offset);

        PathGeometry pathGeometry = new();
        PathFigure pathFigure = new() { StartPoint = start };

        BezierSegment bezierSegment = new(
            new Point(start.X + 50, start.Y), // Control point 1
            new Point(end.X - 50, end.Y),     // Control point 2
            end,                              // End point
            isStroked: true
        );

        pathFigure.Segments.Add(bezierSegment);
        pathGeometry.Figures.Add(pathFigure);
        splinePath.Data = pathGeometry;
    }
    private void Spline_MouseDown(object sender, MouseButtonEventArgs e)
    {
        Gate_MouseDown(sender, e);
        splinePath.Effect = Effect;
    }
    public override void Deselect()
    {
        base.Deselect();
        splinePath.Effect = Effect;
    }
}
