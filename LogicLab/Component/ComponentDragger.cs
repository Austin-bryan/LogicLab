using System.Windows;
using System.Windows.Input;
using LogicLab.Component;

namespace LogicLab;

// Austin (entire class)
public partial class LogicComponent
{
    protected class ComponentDragger(LogicComponent logicComponent)
    {
        // Forward Margin to component. Note, WPF doesn't use positions, they use margins where Margin.Left is X, and Margin.Top is Y.
        private Thickness Margin
        {
            get => logicComponent.Margin;
            set => logicComponent.Margin = value;
        }
        private bool isMouseDown;
        private Point dragStart;
        private readonly LogicComponent logicComponent = logicComponent;
        private bool hasDragStarted = false;
        public event EventHandler? OnDragStarted;
        public event EventHandler? OnDragEnded;

        public void DragStart(MouseEventArgs e)
        {
            dragStart      = logicComponent.PointToScreen(e.GetPosition(logicComponent));
            isMouseDown    = true;
            hasDragStarted = false;

            // This makes it so when all gates are selected, and one is moved, move all of them
            // Certain conditions are in place to avoid an infinite loop
            ComponentSelector.SelectedComponents
                .Where(lc => lc != logicComponent && lc.Dragger != null && lc.Dragger.isMouseDown == false)
                .ToList()
                .ForEach(lc => lc.Dragger?.DragStart(e));
        }
        public void DragMove(MouseEventArgs e)
        {
            if (!isMouseDown)
                return;
            if (!hasDragStarted)
                OnDragStarted?.Invoke(this, e);

            hasDragStarted = true;

            // We have to convert to screen space from the local space to be able to properly drag
            Point mousePos = logicComponent.PointToScreen(e.GetPosition(logicComponent));
            Vector delta   = mousePos - dragStart;
            dragStart      = mousePos;
            Margin         = new(Margin.Left + delta.X, Margin.Top + delta.Y, Margin.Right, Margin.Bottom);

            logicComponent.OnDrag();
        }
        public void DragEnd()
        {
            if (hasDragStarted)
            {
                OnDragEnded?.Invoke(this, EventArgs.Empty);
                hasDragStarted = false;
            }    
            isMouseDown = false;

            // Calls drag end on all selected logic components
            ComponentSelector.SelectedComponents
                .Where(lc => lc != logicComponent && lc.Dragger != null && lc.Dragger.isMouseDown)
                .ToList()
                .ForEach(lc => lc.Dragger?.DragEnd());
        }
    }
}