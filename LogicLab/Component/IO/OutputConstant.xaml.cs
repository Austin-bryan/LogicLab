using System.Windows;
using System.Windows.Controls;

namespace LogicLab;

// Austin
public partial class OutputConstant 
{
    public OutputConstant() : base() => InitializeComponent();
    public OutputConstant(bool signal) : this() => this.signal = signal;

    protected override Grid ControlGrid => Grid;
    private static int count = 0;
    private bool? signal;

    protected override async void Grid_Loaded(object sender, RoutedEventArgs e)
    {
        base.Grid_Loaded(sender, e);

        BackgroundSprite.MouseDown += Component_MouseDown;
        BackgroundSprite.MouseMove += Component_MouseMove;
        BackgroundSprite.MouseUp   += Component_MouseUp;

        Sprite.MouseDown += Component_MouseDown;
        Sprite.MouseMove += Component_MouseMove;
        Sprite.MouseUp   += Component_MouseUp;

        OnLoaded();
        await Task.Delay(100);

        OutputPort.Signal = signal == null ? count++ % 2 == 0 : signal.Value;
        Sprite.Fill = Utilities.GetImage(OutputPort.Signal == true ? "On" : "Off");
    }
}
