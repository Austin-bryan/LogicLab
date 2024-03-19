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
    private IOPort? Output => connectedPorts.TryGetValue(EPortType.Output);
    private IOPort? Input => connectedPorts.TryGetValue(EPortType.Input);
    private Point startPoint;
    private readonly MainWindow mainWindow;

    // The visual for the wire
    private readonly Path splinePath = new()
    {
        Stroke = new SolidColorBrush(Color.FromRgb(200, 200, 200)),
        StrokeThickness = 2
    };
    // The hit detector for the wire
    private readonly Path splineCollider = new()
    {
        Stroke = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)),
        StrokeThickness = 15
    };
    public Wire(MainWindow mainWindow, Point startPoint)
    {
        Dragger = null;     // Dragger is a class that allows logic components to be dragged. Setting it null prevents it from being dragged
        this.mainWindow = mainWindow;
        mainWindow.MainGrid.Children.Insert(0, splinePath); 
        mainWindow.MainGrid.Children.Insert(0, splineCollider);

        // Add events to splines
        splineCollider.MouseDown += Spline_MouseDown;
        splineCollider.MouseUp   += Gate_MouseUp;
        splinePath.MouseDown     += Spline_MouseDown;
        splinePath.MouseUp       += Gate_MouseUp;
        this.startPoint           = startPoint;
        splinePath.Effect         = Effect;         // Gives drop shadow
    }
    public void Remove()
    {
        mainWindow.MainGrid.Children.Remove(splinePath);
        mainWindow.MainGrid.Children.Remove(splineCollider);
    }

    public async void SetEndPoint(EPortType portType, Point endPoint)
    {
        // Pretty sure this 1 millisecond delay is no longer needed.
        //await Task.Delay(1);
        // 

        // Since this is likely no longer going to be async, this function will most likely be deleted,
        // and whatever calls SendEndPoint() will just directly call Draw() instead
        Draw(endPoint);
    }

    public void SetPort(EPortType portType, IOPort ioPort) => connectedPorts.TryAdd(portType, ioPort);

    public void Draw(Point anchor)
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
        double alpha = Math.Min(distance, 500) / 500;
        double controlPointDistance = Lerp(10, 250, alpha * alpha);

        BezierSegment bezierSegment = new(
            new Point(startPoint.X + controlPointDistance, startPoint.Y), // Control point 1
            new Point(endPoint.X - controlPointDistance, endPoint.Y),     // Control point 2
            endPoint,                                 
            isStroked: true
        );

        // Adds bezier to visual
        PathFigure pathFigure = new() { StartPoint = startPoint };
        PathGeometry pathGeometry = new();

        pathFigure.Segments.Add(bezierSegment);
        pathGeometry.Figures.Add(pathFigure);
        splinePath.Data = pathGeometry;
        splineCollider.Data = pathGeometry;
    }
    private static double Lerp(double min, double max, double alpha) => min + (max - min) * Math.Max(0, Math.Min(alpha, 1));
    private static double CalculateDistance(Point p1, Point p2) => Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
    
    private void Spline_MouseDown(object sender, MouseButtonEventArgs e)
    {
        // Highlight effect
        Gate_MouseDown(sender, e);
        splinePath.Effect = Effect;
    }
    public override void Deselect()
    {
        // Return to shadow effect
        base.Deselect();
        splinePath.Effect = Effect;
    }
}