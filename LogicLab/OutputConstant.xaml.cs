using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace LogicLab;
public abstract partial class OutputConstant
{
    protected override Grid ControlGrid => Grid;
    protected override Rectangle BackgroundRect => BackgroundSprite;
    private bool? OutValue = null;

    public OutputConstant() => InitializeComponent();
    protected override void Grid_Loaded(object sender, RoutedEventArgs e)
    {
        OnLoaded();

        Random random = new();
        OutValue = false;
        
        if (random.Next(0, 2) == 1) 
            OutValue = true; // Temporary constant randomization

        OutputPort.Signal = OutValue;
        ChangeColor(OutValue);
    }
    private void ChangeColor(bool? signal)
    {
        Color targetColor = signal == true ? Color.FromRgb(150, 150, 30) : Color.FromRgb(30, 30, 30);
        BackgroundSprite.Fill.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(targetColor, TimeSpan.FromSeconds(0.25)));
    }
}