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
    }
    public void Remove()
    {
        mainWindow.MainGrid.Children.Remove(splinePath);
        mainWindow.MainGrid.Children.Remove(splineCollider);
    }

    public async void SetEndPoint(EPortType portType, Point end)
    {
        await Task.Delay(1);
        mainWindow.DebugLabel.Content = end;
        Draw(portType == EPortType.Input ? EPortType.Output : EPortType.Input, end);
    }

    public void SetPort(EPortType portType, IOPort ioPort)
    {
        connectedPorts.TryAdd(portType, ioPort);
    }

    public void Draw(EPortType portType, Point endPoint)
    {
        const int offset = 25;

        Point startPoint = connectedPorts.ContainsKey(EPortType.Output)
            ? new(Output.EndPoint.X, Output.EndPoint.Y - 3 * Output.Sprite.ActualHeight / 4)
            : new(endPoint.X, endPoint.Y - offset);
            //: new(this.startPoint.X, this.startPoint.Y - offset);
        endPoint = connectedPorts.ContainsKey(EPortType.Input)
            ? new(Input.EndPoint.X, Input.EndPoint.Y - offset)
            : new Point(endPoint.X, endPoint.Y - offset);

        PathGeometry pathGeometry = new();
        PathFigure pathFigure = new() { StartPoint = startPoint };

        BezierSegment bezierSegment = new(
            new Point(startPoint.X + 50, startPoint.Y), // Control point 1
            new Point(endPoint.X - 50, endPoint.Y),     // Control point 2
            endPoint,                                   // End point
            isStroked: true
        );

        pathFigure.Segments.Add(bezierSegment);
        pathGeometry.Figures.Add(pathFigure);
        splinePath.Data = pathGeometry;
        splineCollider.Data = pathGeometry;
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
