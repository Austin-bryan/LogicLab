using System.Collections.Immutable;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace LogicLab;

public static class ComponentSelector
{
    private static readonly List<LogicComponent> selectedComponents = [];
    public static ImmutableList<LogicComponent> SelectedComponents => selectedComponents.ToImmutableList();

    public static Grid Grid
    {
        get => _grid;
        set
        {
            if (_grid == null)
                value.Children.Add(selectionBox);
            _grid ??= value;
        }
    }

    private static readonly Rectangle selectionBox;
    private static bool isMouseDown;
    private static Point mouseDownPos;
    private static Grid _grid;

    static ComponentSelector()
    {
        // Create the selection rectangle
        selectionBox = new Rectangle
        {
            Fill                = new SolidColorBrush(Color.FromArgb(50, 0, 0, 255)), // Transparent blue fill
            Stroke              = Brushes.LightBlue, // Light blue border
            StrokeThickness     = 1,
            Visibility          = Visibility.Collapsed,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment   = VerticalAlignment.Top
        };
    }

    public static void SingleSelect(LogicComponent logicComponent)
    {
        DeselectAll(logicComponent);
        selectedComponents.Add(logicComponent);
    }
    public static void Deselect(LogicComponent logicComponent) => selectedComponents.Remove(logicComponent);
    public static bool IsSelected(LogicComponent logicComponent) => selectedComponents.Contains(logicComponent);
    public static void DeselectAll(LogicComponent? ignore = null)
    {
        for (int i = selectedComponents.Count - 1; i >= 0; i--)
            if (selectedComponents[i] != ignore)
                selectedComponents[i].Deselect();
    }
    public static void ShiftSelect(LogicComponent logicComponent) => selectedComponents.Add(logicComponent);

    // Aligning behaviour
    public static void AlignLeft()
    {
        double min = selectedComponents.Min(lc => lc.Margin.Left);
        selectedComponents.ForEach(lc => lc.SetLeft(min));
        selectedComponents.ForEach(lc => lc.OnDrag(null));
    }
    public static void AlignUp()
    {
        double min = selectedComponents.Min(lc => lc.Margin.Top);
        selectedComponents.ForEach(lc => lc.SetTop(min));
        selectedComponents.ForEach(lc => lc.OnDrag(null));
    }
 
    public static void AlignRight()
    {
        double max = selectedComponents.Max(lc => lc.Margin.Left);
        selectedComponents.ForEach(lc => lc.SetLeft(max));
        selectedComponents.ForEach(lc => lc.OnDrag(null));
    }

    public static void AlignDown()
    {
        double max = selectedComponents.Max(lc => lc.Margin.Top);
        selectedComponents.ForEach(lc => lc.SetTop(max));
        selectedComponents.ForEach(lc => lc.OnDrag(null));
    }


    public static void AlignCenter()
    {
        double maxHeight = selectedComponents.Max(lc => lc.ActualHeight);
        double center = selectedComponents.Where(lc => lc.ActualHeight == maxHeight).ToList()[0].GetTop() + maxHeight / 2;
        selectedComponents.ForEach(lc =>
        {
            double lcCenter = (lc.GetTop() + lc.ActualHeight / 2);
            lc.AddTop(center - lcCenter);
        });
        selectedComponents.ForEach(lc => lc.OnDrag(null));
    }

    public static void AlignMiddle() => throw new NotImplementedException();


    public static void MouseDown(MouseButtonEventArgs e)
    {
        if (e.OriginalSource is not System.Windows.Controls.Grid)
            return;
        // Capture and track the mouse.
        isMouseDown = true;
        mouseDownPos = e.GetPosition(Grid); // Get the mouse position relative to the grid
        Grid.CaptureMouse();

        // Initial placement of the drag selection box.
        selectionBox.Width      = 0;
        selectionBox.Height     = 0;
        selectionBox.Margin     = new(mouseDownPos.X, mouseDownPos.Y, 0, 0);
        selectionBox.Visibility = Visibility.Visible;
    }
    public static void MouseMove(MouseEventArgs e)
    {
        if (!isMouseDown)
            return;

        // When the mouse is held down, reposition the drag selection box.
        Point mousePos = e.GetPosition(Grid);

        if (mouseDownPos.X < mousePos.X)
            selectionBox.Width = mousePos.X - mouseDownPos.X;
        else
        {
            selectionBox.Margin = new(mousePos.X, selectionBox.Margin.Top, 0, 0);
            selectionBox.Width = mouseDownPos.X - mousePos.X;
        }

        if (mouseDownPos.Y < mousePos.Y)
            selectionBox.Height = mousePos.Y - mouseDownPos.Y;
        else
        {
            selectionBox.Margin = new(selectionBox.Margin.Left, mousePos.Y, 0, 0);
            selectionBox.Height = mouseDownPos.Y - mousePos.Y;
        }
    }
    public static void MouseUp(MouseButtonEventArgs e)
    {
        if (!isMouseDown)
            return;
        isMouseDown = false;
        Grid.ReleaseMouseCapture();

        // Hide the drag selection box.
        selectionBox.Visibility = Visibility.Collapsed;

        Point mouseUpPos = e.GetPosition(Grid);
        Rect selectionRect = new(mouseDownPos, mouseUpPos);

        List<LogicComponent> intersectingControls = [];
        FindIntersectingControls(Grid, selectionRect, intersectingControls);

        intersectingControls.ForEach(lc => lc.Select(shiftSelect: true));
    }

    private static void FindIntersectingControls(DependencyObject parent, Rect selectionRect, List<LogicComponent> intersectingControls)
    {
        int count = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is not FrameworkElement element)
                continue;
            
            // Check if the control intersects with the selection rectangle
            Rect bounds = new(element.RenderSize);
            Point topLeft = element.TranslatePoint(new Point(), Grid);
            bounds.Offset(topLeft.X, topLeft.Y);

            if (selectionRect.IntersectsWith(bounds) && element is LogicComponent logicComponent)
                intersectingControls.Add(logicComponent);

            // Recursively search child elements
            FindIntersectingControls(element, selectionRect, intersectingControls);
        }
    }
}