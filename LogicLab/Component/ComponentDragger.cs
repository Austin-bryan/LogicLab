using System.Windows;
using System.Windows.Input;
using LogicLab.Component;

namespace LogicLab;

// Austin (entire class)
// Purpose: Enables dragging for the owning Logic Component.
public partial class LogicComponent
{
    protected class ComponentDragger(LogicComponent owningComponent)
    {
        // Forward Margin to component. Note, WPF doesn't use positions, they use margins where Margin.Left is X, and Margin.Top is Y.
        private Thickness Margin
        {
            get => owningComponent.Margin;
            set => owningComponent.Margin = value;
        }
        private readonly LogicComponent owningComponent = owningComponent;
        private bool isMouseDown, hasDragStarted = false;
        private Point dragStart;
        public event EventHandler? OnDragStarted;
        public event EventHandler? OnDragEnded;

        // When the user has clicked on a component and has moved mouse
        public void DragStart(MouseEventArgs e)
        {
            dragStart      = owningComponent.PointToScreen(e.GetPosition(owningComponent));
            isMouseDown    = true;
            hasDragStarted = false;

            // This makes it so when all gates are selected, and one is moved, move all of them
            // Certain conditions are in place to avoid an infinite loop
            ComponentSelector.SelectedComponents
                .Where(lc => lc != owningComponent && lc.Dragger != null && lc.Dragger.isMouseDown == false)
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
            Point mousePos = owningComponent.PointToScreen(e.GetPosition(owningComponent));
            Vector delta   = mousePos - dragStart;
            dragStart      = mousePos;
            Margin         = new(Margin.Left + delta.X, Margin.Top + delta.Y, Margin.Right, Margin.Bottom);

            owningComponent.RefreshWires();
        }
        public void DragEnd()
        {
            if (hasDragStarted)     // Don't notify of DragEnd if drag was never started to begin with
            {
                OnDragEnded?.Invoke(this, EventArgs.Empty);
                hasDragStarted = false;
            }    
            isMouseDown = false;

            // Calls drag end on all selected logic components
            ComponentSelector.SelectedComponents
                .Where(lc => lc != owningComponent && lc.Dragger != null && lc.Dragger.isMouseDown)
                .ToList()
                .ForEach(lc => lc.Dragger?.DragEnd());
        }
    }
}