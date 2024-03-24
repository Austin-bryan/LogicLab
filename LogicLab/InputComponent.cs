using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace LogicLab;
public abstract class InputComponent : LogicComponent
{
    protected abstract Grid ComponentGrid { get; }
    protected abstract Rectangle BackgroundRect { get; }
    public override void ShowSignal(bool? signal) { }
    protected InputComponent() : base()
    {

    }
    protected void OnLoaded()
    {
        AddOutputPort(ComponentGrid);
        OutputPort.SetLeft(BackgroundRect.ActualWidth / 2 + 14);
    }
}
