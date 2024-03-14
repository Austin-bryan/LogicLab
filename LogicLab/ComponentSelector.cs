using System.Collections.Immutable;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Input;

namespace LogicLab;

public static class ComponentSelector
{
    private static readonly List<LogicComponent> selectedComponents = [];
    public static ImmutableList<LogicComponent> SelectedComponents => selectedComponents.ToImmutableList();

    private static bool isMouseDown;
    private static Point mouseDownPos;
    private static Rectangle selectionBox;

    public static void SingleSelect(LogicComponent logicComponent)
    {
        DeselectAll(logicComponent);
        selectedComponents.Add(logicComponent);
    }
    public static void Deselect(LogicComponent logicComponent)
    {
        selectedComponents.Remove(logicComponent);
    }
    public static bool IsSelected(LogicComponent logicComponent) => selectedComponents.Contains(logicComponent);
    public static void DeselectAll(LogicComponent? ignore = null)
    {
        for (int i = selectedComponents.Count - 1; i >= 0; i--)
            if (selectedComponents[i] != ignore)
                selectedComponents[i].Deselect();
    }
    public static void ShiftSelect(LogicComponent logicComponent)
    {
        selectedComponents.Add(logicComponent);
    }

    public static void MouseDown(MouseButtonEventArgs e)
    {
        if (e.OriginalSource is not Grid)
            return;
        // Capture and track the mouse.
        isMouseDown = true;
        mouseDownPos = e.GetPosition(grid); // Get the mouse position relative to the grid
        grid.CaptureMouse();

        // Initial placement of the drag selection box.
        selectionBox.Width = 0;
        selectionBox.Height = 0;
        selectionBox.Margin = new(mouseDownPos.X, mouseDownPos.Y, 0, 0);
        selectionBox.Visibility = Visibility.Visible;
    }
    public static void MouseMove(MouseEventArgs e)
    {
        if (!isMouseDown)
            return;

        // When the mouse is held down, reposition the drag selection box.
        Point mousePos = e.GetPosition(grid);

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
        isMouseDown = false;
        grid.ReleaseMouseCapture();

        // Hide the drag selection box.
        selectionBox.Visibility = Visibility.Collapsed;

        Point mouseUpPos = e.GetPosition(grid);

        // TODO: 
        //
        // The mouse has been released, check to see if any of the items 
        // in the other canvas are contained within mouseDownPos and 
        // mouseUpPos, for any that are, select them!
        //
    }
}
