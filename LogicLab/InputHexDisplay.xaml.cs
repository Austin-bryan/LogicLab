using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace LogicLab;

public partial class InputHexDisplay
{
    protected override Grid ControlGrid => Grid;
    private IOPort[] ports = new IOPort[4];

    private const int inputCount = 4;
    private double startHeight;

    public InputHexDisplay() => InitializeComponent();
    protected override void Grid_Loaded(object sender, RoutedEventArgs e)
    {
        base.Grid_Loaded(sender, e);
        OnLoaded();
        startHeight = BackgroundSprite.ActualHeight;
    }
    protected override void AddAllInputs()
    {
        for(int i = 0; i < ports.Length; i++)
            ports[i] = AddInputPort(InputPanel);
        BackgroundSprite.Height = startHeight + inputCount * 20;
    }

    public override void ShowSignal(bool? signal)
    {
        base.ShowSignal(signal);
        BinaryAdd(1001);
    }

    public int BinaryAdd(int binaryIn) => Convert.ToInt32(binaryIn.ToString(), 2);
}
