using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace LogicLab;
public partial class InputHexDisplay
{
    IOPort[] ports = new IOPort[4];

    private int inputCount = 4;
    private double startHeight;

    protected override Grid ComponentGrid => Grid;
    protected override Rectangle BackgroundRect => BackgroundSprite;
    public InputHexDisplay() => InitializeComponent();
    protected override void Grid_Loaded(object sender, RoutedEventArgs e)
    {
        OnLoaded();
        startHeight = BackgroundSprite.ActualHeight;
    }
    protected override void AddAllInputs()
    {
        for(int i = 0; i < ports.Length; i++)
        {
            ports[i] = AddInputPort(InputPanel);
        }
        BackgroundSprite.Height = startHeight + inputCount * 20;
    }

    public override void ShowSignal(bool? signal)
    {
        //does this trigger anytime any signal changes?
        Color targetColor = signal == true ? Color.FromRgb(150, 150, 30) : Color.FromRgb(30, 30, 30);
        BackgroundSprite.Fill.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(targetColor, TimeSpan.FromSeconds(0.25)));
        BinaryAdd(1001);
    }

    public int BinaryAdd(int binaryIn) => Convert.ToInt32(binaryIn.ToString(), 2);
}
