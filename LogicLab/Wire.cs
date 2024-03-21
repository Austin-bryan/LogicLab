using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LogicLab;

public partial class Wire : LogicComponent
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
        Stroke = new SolidColorBrush(Color.FromRgb(200, 200, 200)),
        StrokeThickness = 4
    };
    private readonly Path splineCollider = new()
    {
        Stroke = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)),
        StrokeThickness = 15
    };
    
    public Wire(MainWindow mainWindow, Point startPoint)
    {
        Dragger = null;     // Dragger is a class that allows logic components to be dragged. Setting it null prevents it from being dragged
        this.mainWindow = mainWindow;
        this.startPoint = startPoint;

        SetupWire(splineCollider);
        SetupWire(mainSpline);

        void SetupWire(Path path)
        {
            mainWindow.MainGrid.Children.Insert(0, path);
            path.MouseDown += Spline_MouseDown;
            path.MouseUp += Gate_MouseUp;
            path.Effect = Effect;
        }
    }
    public void Remove()
    {
        mainWindow.MainGrid.Children.Remove(mainSpline);
        mainWindow.MainGrid.Children.Remove(splineCollider);
    }
    public void SetPort(EPortType portType, IOPort ioPort) => connectedPorts.TryAdd(portType, ioPort);
    public void Draw(Point anchor, bool? signal = false)
    {
        // Anchor will be either the port position thats connected to the wire, or the mouse position
        const int offset = 25;

        // If a port is null, use the mouse position for the end of a spline. At least one port will be non null.
        // If neither are null, the user has connected this wire to two gates. 
        Point startPoint = new(Output?.WireConnection.X ?? anchor.X, 
                               Output?.WireConnection.Y - 3 * Output?.Sprite.ActualHeight / 4 ?? anchor.Y - offset);
        Point endPoint = new(Input?.WireConnection.X ?? anchor.X, Input?.WireConnection.Y - offset ?? anchor.Y - offset);

        // This sets it so the wire is more bendy as the input and output get further away, improving readability
        double distance = CalculateDistance(startPoint, endPoint);

        // This changes the min and max distance if the wire is going backwards to make it easier to see

        IOPort port = Output ?? Input;
        (Point start, Point end) localized = (port.PointFromScreen(startPoint), port.PointFromScreen(endPoint));

        bool isDraggingFromInput = Output == null && Input != null;

        double backwardsAlpha = isDraggingFromInput ? -localized.start.X / -localized.end.X : -localized.end.X / -localized.start.X;
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
        mainSpline.Data       = pathGeometry;
        splineCollider.Data   = pathGeometry;
    }
    public override void Deselect()
    {
        // Return to shadow effect
        base.Deselect();
        mainSpline.Effect = Effect;
    }

    private static double Lerp(double min, double max, double alpha) => min + (max - min) * Math.Max(0, Math.Min(alpha, 1));
    private static double CalculateDistance(Point p1, Point p2) => Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
    
    private void Spline_MouseDown(object sender, MouseButtonEventArgs e)
    {
        // Highlight effect
        Gate_MouseDown(sender, e);
        mainSpline.Effect = Effect;
        splineCollider.Effect = Effect;
    }
}