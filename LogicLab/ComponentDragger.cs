using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LogicLab;

public partial class LogicComponent
{
    private class ComponentDragger(LogicComponent logicComponent)
    {
        //private Rectangle selectionBox;
        private Thickness Margin
        {
            get => logicComponent.Margin;
            set => logicComponent.Margin = value;
        }
        private bool isMouseDown;
        private Point dragStart;
        private readonly LogicComponent logicComponent = logicComponent;

        public void DragStart(MouseEventArgs e)
        {
            dragStart = logicComponent.PointToScreen(e.GetPosition(logicComponent));
            isMouseDown = true;

            ComponentSelector.SelectedComponents
                .Where(lc => lc != logicComponent && lc.dragger.isMouseDown == false)
                .ToList()
                .ForEach(lc => lc.dragger.DragStart(e));
        }
        public void DragMove(MouseEventArgs e)
        {
            if (!isMouseDown)
                return;

            Point mousePos = logicComponent.PointToScreen(e.GetPosition(logicComponent));
            Vector delta = mousePos - dragStart;
            dragStart = mousePos;
            Margin = new(Margin.Left + delta.X, Margin.Top + delta.Y, Margin.Right, Margin.Bottom);

        }
        public void DragEnd()
        {
            isMouseDown = false;
            ComponentSelector.SelectedComponents
                .Where(lc => lc != logicComponent && lc.dragger.isMouseDown == true)
                .ToList()
                .ForEach(lc => lc.dragger.DragEnd());
        }

        //private readonly Grid grid;
        //public ComponentDragger(Grid grid)
        //{
        //    // Create the selection rectangle
        //    selectionBox = new Rectangle
        //    {
        //        Fill   = new SolidColorBrush(Color.FromArgb(50, 0, 0, 255)), // Transparent blue fill
        //        Stroke = Brushes.LightBlue, // Light blue border
        //        StrokeThickness = 1 // Border thickness
        //    };

        //    // Add the selection rectangle to the canvas
        //    this.grid = grid;
        //    grid.Children.Add(selectionBox);
        //    selectionBox.Visibility          = Visibility.Collapsed;
        //    selectionBox.HorizontalAlignment = HorizontalAlignment.Left;
        //    selectionBox.VerticalAlignment   = VerticalAlignment.Top;
        //}

        //public void MouseDown(MouseButtonEventArgs e)
        //{
        //    if (e.OriginalSource is not Grid)
        //        return;
        //    // Capture and track the mouse.
        //    isMouseDown  = true;
        //    mouseDownPos = e.GetPosition(grid); // Get the mouse position relative to the grid
        //    grid.CaptureMouse();

        //    // Initial placement of the drag selection box.
        //    selectionBox.Width      = 0;
        //    selectionBox.Height     = 0;
        //    selectionBox.Margin     = new(mouseDownPos.X, mouseDownPos.Y, 0, 0);
        //    selectionBox.Visibility = Visibility.Visible;
        //}
        //public void MouseMove(MouseEventArgs e)
        //{
        //    if (!isMouseDown)
        //        return;

        //    // When the mouse is held down, reposition the drag selection box.
        //    Point mousePos = e.GetPosition(grid);

        //    if (mouseDownPos.X < mousePos.X)
        //        selectionBox.Width = mousePos.X - mouseDownPos.X;
        //    else
        //    {
        //        selectionBox.Margin = new(mousePos.X, selectionBox.Margin.Top, 0, 0);
        //        selectionBox.Width = mouseDownPos.X - mousePos.X;
        //    }

        //    if (mouseDownPos.Y < mousePos.Y)
        //        selectionBox.Height = mousePos.Y - mouseDownPos.Y;
        //    else
        //    {
        //        selectionBox.Margin = new(selectionBox.Margin.Left, mousePos.Y, 0, 0);
        //        selectionBox.Height = mouseDownPos.Y - mousePos.Y;
        //    }
        //}
        //public void MouseUp(MouseButtonEventArgs e) 
        //{
        //    isMouseDown = false;
        //    grid.ReleaseMouseCapture();

        //    // Hide the drag selection box.
        //    selectionBox.Visibility = Visibility.Collapsed;

        //    Point mouseUpPos = e.GetPosition(grid);

        //    // TODO: 
        //    //
        //    // The mouse has been released, check to see if any of the items 
        //    // in the other canvas are contained within mouseDownPos and 
        //    // mouseUpPos, for any that are, select them!
        //    //
        //}

    }
}