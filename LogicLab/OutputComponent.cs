using System.Windows.Shapes;
using System.Windows.Controls;

namespace LogicLab;

public abstract class OutputComponent : LogicComponent
{
    protected abstract Rectangle BackgroundRect { get; }

    protected OutputComponent() : base() {  }
    protected void OnLoaded()
    {
        AddOutputPort(ControlGrid);
        OutputPort.SetLeft(BackgroundRect.ActualWidth / 2 + 14);
    }
}
