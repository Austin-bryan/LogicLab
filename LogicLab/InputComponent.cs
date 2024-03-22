
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows;

namespace LogicLab;
public abstract class InputComponent : LogicComponent
{
    protected abstract Grid ComponentGrid { get; }
    protected abstract Rectangle BackgroundRect { get; }
    protected InputComponent() : base()
    {

        //OutputPort.SetLeft()
 
    }
    protected void OnLoaded()
    {
        AddOutputPort(ComponentGrid);
        OutputPort.SetLeft(BackgroundRect.ActualWidth / 2 + 14);
        
    }
}
