using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace LogicLab;

public partial class OutputToggle
{
    protected override Grid ControlGrid => Grid;
    protected override Rectangle BackgroundRect => BackgroundSprite;

    private bool ShouldToggle = false;

    public OutputToggle() => InitializeComponent();
    protected override void Grid_Loaded(object sender, RoutedEventArgs e)
    {
        OnLoaded();
        OutputPort.Signal = false;
    }

    private void Background_MouseDown(object sender, MouseButtonEventArgs e)
    {
        Component_MouseDown(sender, e);
        //TODO: make wire update position when moves
    }

    private void Toggle_MouseDown(object sender, MouseButtonEventArgs e)
    {
        ShouldToggle = true;
        DispatcherTimer timer = new()
        {
            Interval = new TimeSpan(0, 0, 0, 0, 500) // Delays for 500 milliseconds
        };
        timer.Start();
        timer.Tick += (sender, args) =>
        {
            timer.Stop();
            ShouldToggle = false;
        };

        Component_MouseDown(sender, e);
    }

    public override void ShowSignal(bool? signal)
    {
        Color targetColor = signal == true ? Color.FromRgb(150, 150, 30) : Color.FromRgb(30, 30, 30);
        BackgroundSprite.Fill.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(targetColor, TimeSpan.FromSeconds(0.25)));
    }

    private void Toggle_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (ShouldToggle) 
            OutputPort.Signal = !OutputPort.Signal;
        ShowSignal(OutputPort.Signal);

        Component_MouseUp(sender, e);
    }
}
