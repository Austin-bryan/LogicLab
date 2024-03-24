using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace LogicLab;
public abstract class OutputComponent : LogicComponent
{
    protected abstract Grid ComponentGrid { get; }
    protected abstract Rectangle BackgroundRect { get; }
    public abstract void ShowSignal(bool? signal);
    protected OutputComponent() : base()
    {

    }
    protected void OnLoaded()
    {
        AddInputPort(ComponentGrid);
        InputPort.SetLeft(/*-BackgroundRect.ActualWidth / 2*/ - 26);
        InputPort.SetTop(BackgroundRect.ActualHeight / 2 - 8);
    }
    
}
