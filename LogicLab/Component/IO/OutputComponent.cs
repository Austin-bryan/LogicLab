using System.Windows;
using System.Windows.Input;

namespace LogicLab.Component.IO;

// Gary
// Purpose: For classes that only have output ports and no input ports.
public abstract class OutputComponent : LogicComponent
{
    protected OutputComponent() : base() { }

    protected async void OnLoaded()
    {
        await Task.Delay(1);    // AB - Wait for everything to be fully loaded

        AddOutputPort(ControlGrid);
        OutputPort?.SetSignal(false, []);

        (Width, Height) = (75, 50); // AB - Limiting the size fixes a certain selection bug
        OutputPort?.SetLeft(BackgroundSprite.ActualWidth);  // AB
        
        if (OutputPort != null)
            OutputPort.HorizontalAlignment = HorizontalAlignment.Center;
        BackgroundSprite.Cursor = Cursors.SizeAll;
    }
}