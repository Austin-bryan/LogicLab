using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;

namespace LogicLab;
public abstract class InputComponent : LogicComponent
{
    IOPort? defaultInputPort;
    protected abstract Grid ComponentGrid { get; }
    protected abstract Rectangle BackgroundRect { get; }

    protected InputComponent() : base() { }
    protected virtual void AddAllInputs() => defaultInputPort = AddInputPort(ComponentGrid); //virtual void so that a child can change the number on inputs
    protected async void OnLoaded()
    {
        AddAllInputs();
        BackgroundRect.Fill = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0));
        if(defaultInputPort != null)
        {
            InputPort.SetLeft(-BackgroundRect.ActualWidth / 2);
            await Task.Delay(100);
            InputPort.SetTop(BackgroundRect.ActualHeight / 4 + 3);
        }
    }
}