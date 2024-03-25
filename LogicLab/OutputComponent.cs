using System.Windows.Shapes;
using System.Windows.Controls;

namespace LogicLab;

public abstract class OutputComponent : LogicComponent
{
    protected abstract Grid ComponentGrid { get; }
    protected abstract Rectangle BackgroundRect { get; }

    protected OutputComponent() : base() {  }
    protected void OnLoaded()
    {
        AddOutputPort(ComponentGrid);
        OutputPort.SetLeft(BackgroundRect.ActualWidth / 2 + 14);
    }
}
