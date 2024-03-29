using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LogicLab;
public partial class InputHexDisplay
{
    IOPort port1;
    IOPort port2;
    IOPort port3;
    IOPort port4;

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
        port1 = AddInputPort(InputPanel);
        port2 = AddInputPort(InputPanel);
        port3 = AddInputPort(InputPanel);
        port4 = AddInputPort(InputPanel);
        BackgroundSprite.Height = startHeight + inputCount * 20;
    }

    public override void ShowSignal(bool? signal)
    {
        //does this trigger anytime any signal changes?
        Color targetColor = signal == true ? Color.FromRgb(150, 150, 30) : Color.FromRgb(30, 30, 30);
        BackgroundSprite.Fill.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(targetColor, TimeSpan.FromSeconds(0.25)));
    }

    public int BinaryAdd()
    {
        int numInDec = 0;
        //make it do the thing
        return numInDec;
    }
}
