using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LogicLab;

public partial class Wire : LogicComponent
{
    private readonly Dictionary<EPortType, IOPort> connectedPorts = [];
    public ReadOnlyDictionary<EPortType, IOPort> ConnectedPorts => connectedPorts.AsReadOnly();

    public IOPort Output => connectedPorts[EPortType.Output];
    public IOPort Input => connectedPorts[EPortType.Input];
    private Point startPoint;
    private MainWindow mainWindow;

    private readonly Path splinePath = new()
    {
        Stroke = new SolidColorBrush(Color.FromRgb(200, 200, 200)),
        StrokeThickness = 2
    };
    private readonly Path splineCollider = new()
    {
        Stroke = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)),
        StrokeThickness = 15
    };
    public Wire(MainWindow mainWindow, Point startPoint)
    {
        Dragger = null;
        this.mainWindow = mainWindow;
        mainWindow.MainGrid.Children.Insert(0, splinePath);
        mainWindow.MainGrid.Children.Insert(0, splineCollider);

        splineCollider.MouseDown += Spline_MouseDown;
        splineCollider.MouseUp   += Gate_MouseUp;
        splinePath.MouseDown     += Spline_MouseDown;
        splinePath.MouseUp       += Gate_MouseUp;
        this.startPoint           = startPoint;
        splinePath.Effect         = Effect;
        mainWindow.DebugLabel.Content = 0;
    }
    public void Remove()
    {
        mainWindow.MainGrid.Children.Remove(splinePath);
        mainWindow.MainGrid.Children.Remove(splineCollider);
    }

    public async void SetEndPoint(EPortType portType, Point end)
    {
        await Task.Delay(1);
        Draw(portType == EPortType.Input ? EPortType.Output : EPortType.Input, end);
    }

    public void SetPort(EPortType portType, IOPort ioPort) => connectedPorts.TryAdd(portType, ioPort);

    public void Draw(EPortType deleteMe, Point endPoint)
    {
        mainWindow.DebugLabel.Content = (int)(mainWindow.DebugLabel.Content) + 1;
        const int offset = 25;

        Point startPoint = connectedPorts.ContainsKey(EPortType.Output)
            ? new(Output.EndPoint.X, Output.EndPoint.Y - 3 * Output.Sprite.ActualHeight / 4)
            : new(endPoint.X, endPoint.Y - offset);
        endPoint = connectedPorts.ContainsKey(EPortType.Input)
            ? new(Input.EndPoint.X, Input.EndPoint.Y - offset)
            : new Point(endPoint.X, endPoint.Y - offset);

        // This sets it so the wire is more bendy as the input and output get further away, improving readability
        double distance = CalculateDistance(startPoint, endPoint);
        double alpha = Math.Min(distance, 500) / 500;
        double controlPointDistance = Lerp(10, 250, alpha * alpha);

        BezierSegment bezierSegment = new(
            new Point(startPoint.X + controlPointDistance, startPoint.Y), // Control point 1
            new Point(endPoint.X - controlPointDistance, endPoint.Y),     // Control point 2
            endPoint,                                 
            isStroked: true
        );

        PathFigure pathFigure = new() { StartPoint = startPoint };
        PathGeometry pathGeometry = new();

        pathFigure.Segments.Add(bezierSegment);
        pathGeometry.Figures.Add(pathFigure);
        splinePath.Data = pathGeometry;
        splineCollider.Data = pathGeometry;
    }
    public static double Lerp(double min, double max, double alpha)
    {
        alpha = Math.Max(0, Math.Min(alpha, 1));
        return min + (max - min) * alpha;
    }
    private static double CalculateDistance(Point p1, Point p2)
    {
        return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
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