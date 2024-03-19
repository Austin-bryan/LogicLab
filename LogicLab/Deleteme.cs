using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace LogicLab;

public class Deleteme(MainWindow mainWindow) : FrameworkElement
{
    private List<SplineData> splineDataList = [];

    public void AddSpline(Point startPoint, Point endPoint)
    {
        splineDataList.Add(new(startPoint, endPoint));
    }
    public class VisualHost : UIElement
    {
        private DrawingVisual drawingVisual;

        public VisualHost(DrawingVisual visual)
        {
            drawingVisual = visual;
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index != 0)
                throw new ArgumentOutOfRangeException();

            return drawingVisual;
        }
    }

    public void DrawSplinesBatch()
    {
        DrawingVisual drawingVisual = new();

        using (DrawingContext drawingContext = drawingVisual.RenderOpen())
        {
            const int offset = 25;

            foreach (SplineData splineData in splineDataList)
            {
                Point startPoint = splineData.StartPoint;
                Point endPoint = splineData.EndPoint;

                PathGeometry pathGeometry = new ();
                PathFigure pathFigure = new() { StartPoint = startPoint };

                double distance = CalculateDistance(startPoint, endPoint);
                double alpha = Math.Min(distance, 500) / 500;
                double controlPointDistance = Lerp(10, 250, alpha * alpha);

                BezierSegment bezierSegment = new BezierSegment(
                    new Point(startPoint.X + controlPointDistance, startPoint.Y), // Control point 1
                    new Point(endPoint.X - controlPointDistance, endPoint.Y),     // Control point 2
                    endPoint,
                    isStroked: true
                );

                pathFigure.Segments.Add(bezierSegment);
                pathGeometry.Figures.Add(pathFigure);

                // Draw each spline onto the DrawingContext
                drawingContext.DrawGeometry(null, new Pen(Brushes.Black, 2), pathGeometry);
            }
        }

        // Add the DrawingVisual to the visual tree
        VisualHost vs = new(drawingVisual);
        mainWindow.MainGrid.Children.Add(vs);
        splineDataList.Clear();
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        const int offset = 25;

        foreach (SplineData splineData in splineDataList)
        {
            Point startPoint = splineData.StartPoint;
            Point endPoint = splineData.EndPoint;

            PathGeometry pathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure() { StartPoint = startPoint };

            double distance = CalculateDistance(startPoint, endPoint);
            double alpha = Math.Min(distance, 500) / 500;
            double controlPointDistance = Lerp(10, 250, alpha * alpha);

            BezierSegment bezierSegment = new(
                new Point(startPoint.X + controlPointDistance, startPoint.Y), // Control point 1
                new Point(endPoint.X - controlPointDistance, endPoint.Y),     // Control point 2
                endPoint,
                isStroked: true
            );

            pathFigure.Segments.Add(bezierSegment);
            pathGeometry.Figures.Add(pathFigure);

            // Draw each spline onto the DrawingContext
            drawingContext.DrawGeometry(null, new Pen(Brushes.Black, 2), pathGeometry);
        }
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
    // Helper class to hold spline data
    private record SplineData(Point StartPoint, Point EndPoint);
}