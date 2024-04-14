using System.Windows;
using System.Windows.Input;

namespace LogicLab.Component.IO;

// Gary
public abstract class OutputComponent : LogicComponent
{
    protected OutputComponent() : base() { }

    // TODO:: remove onloaded and replace with Grid_Loaded
    protected async void OnLoaded()
    {
        // TODO:: Find better solution
        await Task.Delay(1);

        AddOutputPort(ControlGrid);
        OutputPort?.SetSignal(false, []);

        (Width, Height) = (75, 50);
        OutputPort?.SetLeft(BackgroundSprite.ActualWidth);
        
        if (OutputPort != null)
            OutputPort.HorizontalAlignment = HorizontalAlignment.Center;
        BackgroundSprite.Cursor = Cursors.SizeAll;
    }
}