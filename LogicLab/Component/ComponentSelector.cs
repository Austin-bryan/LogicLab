﻿using System.Collections.Immutable;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LogicLab;

namespace LogicLab.Component;
 
// AB (entire class)
// Purpose: Allows selection of logic component, and handles multiselection
public static class ComponentSelector
{
    private static readonly List<LogicComponent> selectedComponents = [];
    public static ImmutableList<LogicComponent> SelectedComponents => selectedComponents.ToImmutableList();

    // Only set MainGrid once, while it's null
    public static Grid MainGrid
    {
        get => _mainGrid ?? throw new ArgumentNullException(nameof(_mainGrid));
        set
        {
            if (_mainGrid == null)
                value.Children.Add(selectionBox);
            _mainGrid ??= value;
        }
    }
    private static Label DebugLabel => ((MainWindow)Window.GetWindow(MainGrid)).DebugLabel;
    
    private static readonly Rectangle selectionBox;
    private static bool isMouseDown, hasDragged;
    private static Point mouseDownPos;
    private static Grid? _mainGrid;

    static ComponentSelector()
    {
        selectionBox = new Rectangle    // Used to select logic components
        {
            Fill                = new SolidColorBrush(Color.FromArgb(50, 0, 0, 255)),
            Stroke              = Brushes.LightBlue,
            Visibility          = Visibility.Collapsed,
            StrokeThickness     = 1,
            VerticalAlignment   = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Left,
        };
    }


    public static void SingleSelect (LogicComponent logicComponent)
    {
        DeselectAll(logicComponent);
        selectedComponents.Add(logicComponent);
    }
    public static void Deselect     (LogicComponent logicComponent) => selectedComponents.Remove(logicComponent);
    public static bool IsSelected   (LogicComponent logicComponent) => selectedComponents.Contains(logicComponent);
    public static void ShiftSelect  (LogicComponent logicComponent) => selectedComponents.Add(logicComponent);
    public static void DeselectAll  (LogicComponent? ignore = null)
    {
        for (int i = selectedComponents.Count - 1; i >= 0; i--)
            if (selectedComponents[i] != ignore)
                selectedComponents[i].Deselect();
    }

    // Aligning behaviour
    public static async void AlignLeft()
    {
        double min = selectedComponents.Min(lc => lc.Margin.Left);
        selectedComponents.ForEach(lc => lc.SetLeft(min));
        await Task.Delay(10);
        selectedComponents.ForEach(lc => lc.RefreshWires());
    }
    public static async void AlignTop()
    {
        double min = selectedComponents.Min(lc => lc.Margin.Top);
        selectedComponents.ForEach(lc => lc.SetTop(min));
        await Task.Delay(10);
        selectedComponents.ForEach(lc => lc.RefreshWires());
    }
    public static async void AlignRight()
    {
        double max = selectedComponents.Max(lc => lc.Margin.Left);
        selectedComponents.ForEach(lc => lc.SetLeft(max));
        await Task.Delay(10);
        selectedComponents.ForEach(lc => lc.RefreshWires());
    }
    public static async void AlignBottom()
    {
        double max = selectedComponents.Max(lc => lc.Margin.Top);
        selectedComponents.ForEach(lc => lc.SetTop(max));
        await Task.Delay(10);
        selectedComponents.ForEach(lc => lc.RefreshWires());
    }
    public static async void AlignCenter()
    {
        double maxHeight = selectedComponents.Max(lc => lc.ActualHeight);
        double center = selectedComponents.Where(lc => lc.ActualHeight == maxHeight).ToList()[0].GetTop() + maxHeight / 2;
        selectedComponents.ForEach(lc =>
        {
            double lcCenter = lc.GetTop() + lc.ActualHeight / 2;
            lc.AddTop(center - lcCenter);
        });
        await Task.Delay(10);
        selectedComponents.ForEach(lc => lc.RefreshWires());
    }

    public static void MouseDown(MouseButtonEventArgs e)
    {
        if (e.OriginalSource != MainGrid)
            return;
        // Capture and track the mouse.
        isMouseDown  = true;
        mouseDownPos = e.GetPosition(MainGrid); // Get the mouse position relative to the grid
        MainGrid.CaptureMouse();

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

        hasDragged = true;

        // When the mouse is held down, reposition the drag selection box.
        Point mousePos = e.GetPosition(MainGrid);

        if (mouseDownPos.X < mousePos.X)
            selectionBox.Width = mousePos.X - mouseDownPos.X;
        else
        {
            selectionBox.Margin = new(mousePos.X, selectionBox.Margin.Top, 0, 0);
            selectionBox.Width  = mouseDownPos.X - mousePos.X;
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

        MainGrid.ReleaseMouseCapture();
        isMouseDown = false;

        if (!hasDragged)    // Prevents accidental selection
            return;
        
        hasDragged = false;
        selectionBox.Visibility = Visibility.Collapsed;

        // Get bounds of selection search
        Point mouseUpPos = e.GetPosition(MainGrid);
        Rect selectionRect = new(mouseDownPos, mouseUpPos);

        List<LogicComponent> intersectingControls = [];
        FindIntersectingControls(MainGrid, selectionRect, intersectingControls);

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
            Rect bounds   = new(element.RenderSize);
            Point topLeft = element.TranslatePoint(new Point(), MainGrid);
            bounds.Offset(topLeft.X, topLeft.Y);

            if (selectionRect.IntersectsWith(bounds) && element is LogicComponent logicComponent)
                intersectingControls.Add(logicComponent);

            // Recursively search child elements
            FindIntersectingControls(element, selectionRect, intersectingControls);
        }
    }

    // Connor
    public static void DeleteComponent() => SelectedComponents.ForEach(c =>
    {
        c.OnDelete();   // AB
        MainGrid.Children.Remove(c);
    });
}