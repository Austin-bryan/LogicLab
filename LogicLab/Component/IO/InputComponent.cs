using System.Windows.Input;

namespace LogicLab.Component.IO;

// Gary
// Class purpose: Base class for all logic components that only have inputs, no outputs. 
// These classes are used to results of logical equations
public abstract class InputComponent : LogicComponent
{
    protected InputComponent() : base() { }

    // Gary
    protected virtual async void AddAllInputs()
    {
        AddInputPort(ControlGrid);
        await Task.Delay(1);    // Waits for Input Port to be fully, before setting its position
        InputPort?.SetLeft(-BackgroundSprite.ActualWidth / 2);
        InputPort?.SetTop(BackgroundSprite.ActualHeight / 4 + 3);
    }

    // Austin
    protected async void OnLoaded()
    {
        await Task.Delay(10);
        AddAllInputs();

        // Setup Background sprite
        BackgroundSprite.MouseLeftButtonDown += Component_MouseLeftButtonDown;
        BackgroundSprite.MouseLeftButtonUp   += Component_MouseLeftButtonUp;
        BackgroundSprite.MouseMove           += Component_MouseMove;
        BackgroundSprite.Cursor               = Cursors.SizeAll;
    }
}