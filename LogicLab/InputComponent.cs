using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;

namespace LogicLab;
public abstract class InputComponent : LogicComponent
{
    protected abstract Rectangle BackgroundRect { get; }

    protected InputComponent() : base() { }
    protected async void OnLoaded()
    {
        AddInputPort(ControlGrid);
        BackgroundRect.Fill = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0));
        InputPort.SetLeft(-BackgroundRect.ActualWidth / 2);
        await Task.Delay(100);
        InputPort.SetTop(BackgroundRect.ActualHeight / 4 + 3);
    }
}