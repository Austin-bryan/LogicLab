using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;

namespace LogicLab;
public abstract class InputComponent : LogicComponent
{
    protected InputComponent() : base() { }
    protected async void OnLoaded()
    {
        await Task.Delay(100);
        AddInputPort(ControlGrid);
        InputPort.SetLeft(-BackgroundSprite.ActualWidth / 2);
        InputPort.SetTop(BackgroundSprite.ActualHeight / 4 + 3);
    }
}