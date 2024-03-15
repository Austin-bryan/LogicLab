using System.Windows;
using System.Windows.Input;

namespace LogicLab;

public partial class LogicComponent
{
    private class ComponentDragger(LogicComponent logicComponent)
    {
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
            return;
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
    }
}