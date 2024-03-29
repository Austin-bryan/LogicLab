using System.Windows.Input;

namespace LogicLab;

public abstract class InputComponent : LogicComponent
{
    protected InputComponent() : base() { }
    protected async void OnLoaded()
    {
        await Task.Delay(10);
        AddInputPort(ControlGrid);
        InputPort.SetLeft(-BackgroundSprite.ActualWidth / 2);
        InputPort.SetTop(BackgroundSprite.ActualHeight / 4 + 3);

        BackgroundSprite.MouseDown += Component_MouseDown;
        BackgroundSprite.MouseMove += Component_MouseMove;
        BackgroundSprite.MouseUp   += Component_MouseUp;
        BackgroundSprite.Cursor = Cursors.SizeAll;
    }
}