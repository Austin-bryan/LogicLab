using System.Windows;
using System.Windows.Controls;

namespace LogicLab;

// Gary
public partial class InputHexDisplay
{
    // Austin
    protected override Grid ControlGrid => Grid;
    private readonly IOPort[] ports = new IOPort[4];

    private const int inputCount = 4;
    private double startHeight;

    public InputHexDisplay() => InitializeComponent();
    protected override void Grid_Loaded(object sender, RoutedEventArgs e)
    {
        base.Grid_Loaded(sender, e);
        OnLoaded();
        startHeight = BackgroundSprite.ActualHeight;    // Gary
        ShowSignal(null);   // Show null display
    }
    // Gary
    protected override void AddAllInputs()
    {
        for(int i = 0; i < ports.Length; i++)
            ports[i] = AddInputPort(InputPanel);
        BackgroundSprite.Height = startHeight + inputCount * 20;
    }
    // Austin
    public override void ShowSignal(bool? signal)
    {
        base.ShowSignal(signal);

        if (signal == null)
        {
            HexDisplayLabel.Visibility = Visibility.Hidden;
            NullDot.Visibility = Visibility.Visible;
        }
        else
        {
            HexDisplayLabel.Visibility = Visibility.Visible;
            NullDot.Visibility = Visibility.Hidden;

            HexDisplayLabel.Content = ports.Select((port, index) => port.Signal == true ? 1 << index : 0)
                                           .Sum().ToString("X");
        }

    }
}