﻿using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Collections.ObjectModel;

namespace LogicLab.Component;

// Austin (entire class)
public partial class Wire : LabComponent
{
    private readonly Dictionary<EPortType, IOPort> connectedPorts = [];                         // Each wire is connected to two ports
    public ReadOnlyDictionary<EPortType, IOPort> ConnectedPorts => connectedPorts.AsReadOnly(); // This prevents public access from mutating the list

    // If there is a connected port, return it, else return null.
    // An input port is missing if the user is dragging from the output port
    // but hasn't selected an input port yet
    public IOPort? Output => connectedPorts.TryGetValue(EPortType.Output);
    public IOPort? Input => connectedPorts.TryGetValue(EPortType.Input);
    private Point startPoint;
    private readonly MainWindow mainWindow;

    // The visual for the wire
    private readonly SolidColorBrush offBrush   = new(Color.FromRgb(200, 200, 200)),
                                     onBrush    = new(Color.FromRgb(200, 200, 0)),
                                     errorBrush = new(Color.FromRgb(200, 50, 50));
    private readonly Path mainSpline = new()
    {
        Stroke           = new SolidColorBrush(Color.FromRgb(200, 200, 200)),
        StrokeThickness  = 4,
        IsHitTestVisible = false
    };

    public Wire(MainWindow mainWindow, Point startPoint)
    {
        this.mainWindow = mainWindow;
        this.startPoint = startPoint;
        SetupWire(mainSpline);

        void SetupWire(Path path)
        {
            mainWindow.MainGrid.Children.Insert(0, path);
            path.MouseDown += Spline_MouseDown;
            path.Effect = Effect;
        }
    }

    public void Remove(bool updateSprite = true)
    {
        mainWindow.MainGrid.Children.Remove(mainSpline);
        Input?.RemoveWire(this, updateSprite);
        Output?.RemoveWire(this);
    }
    public void SetPort(EPortType portType, IOPort ioPort) => connectedPorts.TryAdd(portType, ioPort);
    public void Draw(Point anchor, bool? signal = false)
    {
        // Anchor will be either the port position thats connected to the wire, or the mouse position
        const int offset = 25;
        Point startPoint = new(Output?.WireConnection.X ?? anchor.X,
                               Output?.WireConnection.Y - 3 * Output?.Sprite.ActualHeight / 4 ?? anchor.Y - offset);
        Point endPoint   = new(Input?.WireConnection.X ?? anchor.X, Input?.WireConnection.Y - offset ?? anchor.Y - offset);
        double distance  = CalculateDistance(startPoint, endPoint);
        IOPort port      = Output ?? Input ?? throw new NullReferenceException("Wire has no ports attached");

        if (port == null)
            return;
        if (PresentationSource.FromVisual(port) == null)
            return;

        (Point start, Point end) = (port.PointFromScreen(startPoint), port.PointFromScreen(endPoint));

        bool isDraggingFromInput = Output == null && Input != null;

        double backwardsAlpha = isDraggingFromInput ? -start.X / -end.X : -end.X / -start.X;
        backwardsAlpha = Math.Min(backwardsAlpha, 2.0);
        backwardsAlpha = Math.Max(backwardsAlpha, 0.0);

        if (isDraggingFromInput)
            backwardsAlpha = 2 - backwardsAlpha;

        double maxDistance      = Lerp(2000, 500, backwardsAlpha);
        double minDistance      = Lerp(100, 20, backwardsAlpha);
        double alpha            = Math.Min(distance, maxDistance) / maxDistance;
        double controlPointDist = Lerp(minDistance, maxDistance / 2, alpha * alpha);

        BezierSegment bezierSegment = new(
            new Point(startPoint.X + controlPointDist, startPoint.Y), // Control point 1
            new Point(endPoint.X - controlPointDist, endPoint.Y),     // Control point 2
            endPoint,
            isStroked: true
        );

        // Adds bezier to visual
        PathFigure pathFigure = new() { StartPoint = startPoint };
        PathGeometry pathGeometry = new();

        pathFigure.Segments.Add(bezierSegment);
        pathGeometry.Figures.Add(pathFigure);

        mainSpline.Stroke = signal == true
            ? onBrush
            : signal == false || isDraggingFromInput
            ? offBrush
            : errorBrush;
        mainSpline.Visibility = Visibility.Visible;
        mainSpline.Data = pathGeometry;
    }

    public override void ShowSignal(bool? signal)
    {
        Color color = signal == true
            ? onBrush.Color
            : signal == false
            ? Color.FromRgb(200, 200, 200)
            : Color.FromRgb(200, 50, 50);
        mainSpline.Stroke = new SolidColorBrush(color);
    }

    private static double Lerp(double min, double max, double alpha) => min + (max - min) * Math.Max(0, Math.Min(alpha, 1));
    private static double CalculateDistance(Point p1, Point p2) => Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));

    private void Spline_MouseDown(object sender, MouseButtonEventArgs e) => mainSpline.Effect = Effect;
}