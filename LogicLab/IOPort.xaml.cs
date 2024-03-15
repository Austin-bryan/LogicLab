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
    // TODO:: Remove dependency prop
    public static readonly DependencyProperty PortTypeProperty =
        DependencyProperty.Register("PortType", typeof(EPortType), typeof(IOPort), new PropertyMetadata(EPortType.Input));

    public EPortType PortType
    {
        get => (EPortType)GetValue(PortTypeProperty);
        set => SetValue(PortTypeProperty, value);
    }
    private Polyline splineCurve = new()
    {
        Stroke             = Brushes.Red,
        StrokeThickness    = 3,
        StrokeStartLineCap = PenLineCap.Round,
        StrokeEndLineCap   = PenLineCap.Round
    };
    private List<Point> controlPoints = [];


    public Grid MainGrid => (Window.GetWindow(this) as MainWindow)?.MainGrid ?? throw new NullReferenceException("Null ref");
    public Label DebugLabel => (Window.GetWindow(this) as MainWindow)?.DebugLabel ?? throw new NullReferenceException("Null ref");

    private readonly SolidColorBrush idleColor, hoverColor;
    private bool isPressed;
    private int splineResolution = 10;

    public IOPort(EPortType portType)
    {
        InitializeComponent();

        idleColor  = new SolidColorBrush(Color.FromRgb(9, 180, 255));
        hoverColor = new SolidColorBrush(Color.FromRgb(59, 230, 255));
        PortType   = portType;

        BitmapImage bitmapImage = new(new Uri($"Images/{PortType}.png", UriKind.Relative));
        ImageBrush imageBrush = new(bitmapImage);

        Sprite.Fill = idleColor;
        Sprite.OpacityMask = imageBrush;

        if (portType == EPortType.Input)
            return;

        // You may need to adjust the hover size as per your requirements
        Sprite.Height *= 2.5;
        Sprite.Width *= 1.75;
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
        controlPoints[1] = PointToScreen(e.GetPosition(this));
        DrawSpline();
    }
    private void Wire_MouseUp(object sender, MouseEventArgs e)
    {
        isPressed = false;
        MainGrid.MouseMove -= Wire_MouseMove;
        MainGrid.MouseUp -= Wire_MouseUp;
    }

    private void Sprite_MouseDown(object sender, MouseEventArgs e)
    {
        isPressed = true;
        MainGrid.MouseMove += Wire_MouseMove;
        MainGrid.MouseUp += Wire_MouseUp;

        controlPoints.Clear();
        controlPoints.Add(PointToScreen(e.GetPosition(this)));
        controlPoints.Add(PointToScreen(e.GetPosition(this)));
    }

    private Point BezierInterpolation(Point p0, Point p1, double t)
    {
        double x = (1 - t) * p0.X + t * p1.X;
        double y = (1 - t) * p0.Y + t * p1.Y;
        return new Point(x, y);
    }

    private List<Point> CalculateBezierSplinePoints(List<Point> points)
    {
        List<Point> splinePoints = new List<Point>();

        if (points.Count < 2)
            return splinePoints;

        for (int i = 0; i <= splineResolution; i++)
        {
            double t = (double)i / splineResolution;
            Point interpolatedPoint = BezierInterpolation(points[0], points[1], t);
            splinePoints.Add(interpolatedPoint);
        }

        return splinePoints;
    }
    private void DrawSpline()
    {
        // Clear the previous spline curve
        splineCurve.Points.Clear();

        // Check if there are enough control points to draw the spline
        if (controlPoints.Count < 2)
            return;

        // Interpolate spline points using Bezier interpolation
        List<Point> splinePoints = CalculateBezierSplinePoints(controlPoints);

        // Add the interpolated points to the spline curve
        foreach (Point point in splinePoints)
            splineCurve.Points.Add(point);

        // Invalidate the visual of the spline curve to force a refresh
        splineCurve.InvalidateVisual();
    }
    //private void DrawSpline()
    //{
    //    // Clear the previous spline curve
    //    splineCurve.Points.Clear();

    //    // Check if there are enough control points to draw the spline
    //    if (controlPoints.Count < 2)
    //        return;

    //    // Interpolate spline points using a spline algorithm
    //    List<Point> splinePoints = CalculateSplinePoints(controlPoints);

    //    // Add the interpolated points to the spline curve
    //    foreach (Point point in splinePoints)
    //        splineCurve.Points.Add(point);

    //    splineCurve.InvalidateVisual();
    //}
    private List<Point> CalculateSplinePoints(List<Point> points)
    {
        List<Point> splinePoints = new List<Point>();

        // Ensure there are at least two control points
        if (points.Count < 2)
            return splinePoints;

        // Add the first control point
        splinePoints.Add(points[0]);

        // Interpolate spline points between each pair of control points
        for (int i = 0; i < points.Count - 1; i++)
        {
            Point p0 = i > 0 ? points[i - 1] : points[i];
            Point p1 = points[i];
            Point p2 = points[i + 1];
            Point p3 = i < points.Count - 2 ? points[i + 2] : points[i + 1];

            for (int j = 1; j <= splineResolution; j++)
            {
                double t = (double)j / splineResolution;
                Point interpolatedPoint = CatmullRomInterpolation(p0, p1, p2, p3, t);
                splinePoints.Add(interpolatedPoint);
            }
        }

        // Add the last control point
        splinePoints.Add(points[^1]);

        return splinePoints;
    }

    private void Grid_Loaded(object sender, RoutedEventArgs e)
    {
        MainGrid.Children.Add(splineCurve);

    }

    private static Point CatmullRomInterpolation(Point p0, Point p1, Point p2, Point p3, double t)
    {
        double t2 = t * t;
        double t3 = t2 * t;

        double b0 = 0.5 * (-t3 + 2 * t2 - t);
        double b1 = 0.5 * (3 * t3 - 5 * t2 + 2);
        double b2 = 0.5 * (-3 * t3 + 4 * t2 + t);
        double b3 = 0.5 * (t3 - t2);

        double x = b0 * p0.X + b1 * p1.X + b2 * p2.X + b3 * p3.X;
        double y = b0 * p0.Y + b1 * p1.Y + b2 * p2.Y + b3 * p3.Y;

        return new Point(x, y);
    }
}