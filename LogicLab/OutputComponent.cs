using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace LogicLab;
public abstract class OutputComponent : LogicComponent
{
    protected abstract Grid ComponentGrid { get; }
    protected abstract Rectangle BackgroundRect { get; }
    protected OutputComponent() : base()
    {

    }
    protected void OnLoaded()
    {
        AddInputPort(ComponentGrid);
        InputPort.SetRight(BackgroundRect.ActualWidth / 2 + 14);
    }
}
