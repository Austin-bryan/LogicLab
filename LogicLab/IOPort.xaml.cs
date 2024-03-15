using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Shapes;

namespace LogicLab;

public enum EPortType { Input, Output }

public partial class IOPort : UserControl
{
    private EPortType portType;

    private readonly SolidColorBrush idleColor, hoverColor;
    private bool isPressed;
    private Point startPoint;
    private Path splinePath = new()
    {
        Stroke = new SolidColorBrush(Color.FromRgb(200, 200, 200)),
        StrokeThickness = 5
    };
    private Path splineBorder = new()
    {
        Stroke = new SolidColorBrush(Color.FromRgb(10, 10, 10)),
        StrokeThickness = 10
    };
    private Wire wire;

    public IOPort(EPortType portType)
    {
        InitializeComponent();

        idleColor     = new SolidColorBrush(Color.FromRgb(9, 180, 255));
        hoverColor    = new SolidColorBrush(Color.FromRgb(59, 230, 255));
        this.portType = portType;

        BitmapImage bitmapImage = new(new Uri($"Images/{this.portType}.png", UriKind.Relative));
        ImageBrush imageBrush = new(bitmapImage);

        Sprite.Fill = idleColor;
        Sprite.OpacityMask = imageBrush;

        if (portType == EPortType.Input)
            return;

        // You may need to adjust the hover size as per your requirements
        Sprite.Height *= 2.5;
        Sprite.Width *= 1.75;
    }
    private void DrawSmoothSpline(Point start, Point end)
    {
        const int offset = 25;

        start = new Point(start.X, start.Y - offset); 
        end = new Point(end.X, end.Y - offset); 

        PathGeometry pathGeometry = new();
        PathFigure pathFigure = new();

        pathFigure.StartPoint = start;

        BezierSegment bezierSegment = new BezierSegment(
            new Point(start.X + 50, start.Y), // Control point 1
            new Point(end.X - 50, end.Y),     // Control point 2
            end,                               // End point
            true                                // IsStroked
        );

        pathFigure.Segments.Add(bezierSegment);
        pathGeometry.Figures.Add(pathFigure);
        splinePath.Data = pathGeometry;
        splineBorder.Data = pathGeometry;
    }

    private void Sprite_MouseEnter(object sender, MouseEventArgs e)
    {
        Sprite.Fill = hoverColor;
    }

    private void Sprite_MouseLeave(object sender, MouseEventArgs e)
    {
        Sprite.Fill = idleColor;
    }

    private void Wire_MouseMove(object sender, MouseEventArgs e)
    {
        if (!isPressed) 
            return;

        wire.Draw(PointToScreen(e.GetPosition(this)));
        //DrawSmoothSpline(startPoint, PointToScreen(e.GetPosition(this)));
    }
    private void Wire_MouseUp(object sender, MouseEventArgs e)
    {
        isPressed = false;
        this.MainGrid().MouseMove -= Wire_MouseMove;
        this.MainGrid().MouseUp -= Wire_MouseUp;
    }

    private void Sprite_MouseDown(object sender, MouseEventArgs e)
    {
        isPressed = true;
        this.MainGrid().MouseMove += Wire_MouseMove;
        this.MainGrid().MouseUp += Wire_MouseUp;

        startPoint = PointToScreen(e.GetPosition(this));

        wire = new Wire(this.MainWindow(), startPoint);
        wire.Draw(PointToScreen(e.GetPosition(this)));
        //DrawSmoothSpline(startPoint, startPoint);

    }
    private void ClearSpline()
    {
        if (splinePath != null && this.MainGrid().Children.Contains(splinePath))
        {
            this.MainGrid().Children.Remove(splinePath);
        }
    }

    private void Grid_Loaded(object sender, RoutedEventArgs e)
    {
        this.MainGrid().Children.Insert(0, splinePath);
        this.MainGrid().Children.Insert(0, splineBorder);

    }
}