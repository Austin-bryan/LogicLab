using System.Windows;
using System.Windows.Controls;

namespace LogicLab;

// Gary
// Purpose: Outputs a hex number from 0 to F by treating its 4 inputs as bits.
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
        ShowSignal(null);   // AB - Show null display
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

        // Show a dot if can't show a valid number
        if (signal == null)
        {
            HexDisplayLabel.Visibility = Visibility.Hidden;
            NullDot.Visibility = Visibility.Visible;
        }
        else
        {
            HexDisplayLabel.Visibility = Visibility.Visible;
            NullDot.Visibility = Visibility.Hidden;

            // Bit shifts all input ports based on their index, then sums them all to get a hex value
            HexDisplayLabel.Content = ports.Select((port, index) => port.GetSignal() == true ? 1 << index : 0)
                                           .Sum().ToString("X");
        }
    }
}