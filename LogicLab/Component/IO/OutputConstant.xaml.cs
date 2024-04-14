using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace LogicLab;

// Austin
public partial class OutputConstant 
{
    protected override Rectangle ForegroundSprite => Sprite;
    public OutputConstant() : base() => InitializeComponent();
    public OutputConstant(bool signal) : this() => this.signal = signal;

    protected override Grid ControlGrid => Grid;
    private readonly bool? signal;

    protected override async void Grid_Loaded(object sender, RoutedEventArgs e)
    {
        base.Grid_Loaded(sender, e);

        BackgroundSprite.MouseLeftButtonDown += Component_MouseLeftButtonDown;
        BackgroundSprite.MouseLeftButtonUp   += Component_MouseLeftButtonUp;
        BackgroundSprite.MouseMove += Component_MouseMove;

        OnLoaded();
        await Task.Delay(100);

        OutputPort?.SetSignal(signal ?? false, []);
        Sprite.Fill = Utilities.GetImage(OutputPort?.GetSignal() == true ? "On" : "Off");
    }
}
