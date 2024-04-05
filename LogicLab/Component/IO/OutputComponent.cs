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
        OutputPort.SetLeft(BackgroundSprite.ActualWidth * 2);
        OutputPort.Signal = false;
        BackgroundSprite.Cursor = Cursors.SizeAll;
    }
}
