namespace LogicLab;

public abstract class OutputComponent : LogicComponent
{
    protected OutputComponent() : base() {  }
    protected async void OnLoaded()
    {
        // TODO:: Find better solution
        await Task.Delay(1);
        AddOutputPort(ControlGrid);
        OutputPort.SetLeft(BackgroundSprite.ActualWidth * 2);
        OutputPort.Signal = false;
    }
}
