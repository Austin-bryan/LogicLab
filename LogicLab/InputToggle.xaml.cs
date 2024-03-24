using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.Windows.Media;
namespace LogicLab;

public partial class InputToggle
{
    protected override Grid ComponentGrid => Grid;
    protected override Rectangle BackgroundRect => BackgroundSprite;

    private bool ShouldToggle = false;

    public InputToggle()
    {
        InitializeComponent();
    }
    protected override void Grid_Loaded(object sender, RoutedEventArgs e)
    {
        OnLoaded();
        OutputPort.Signal = false;
    }

    private void Background_MouseDown(object sender, MouseButtonEventArgs e)
    {
        Gate_MouseDown(sender, e);
        //TODO: make wire update position when moves
    }

    private void Toggle_MouseDown(object sender, MouseButtonEventArgs e)
    {
        ShouldToggle = true;
        DispatcherTimer timer = new();
        timer.Interval = new TimeSpan(0, 0, 0, 0, 200);//delays for 2 milliseconds
        timer.Start();
        timer.Tick += (sender, args) =>
        {
            timer.Stop();
            ShouldToggle = false;
        };

        Gate_MouseDown(sender, e);
    }

    private void ChangeColor(bool? signal)
    {
        Color targetColor = signal == true ? Color.FromRgb(150, 150, 30) : Color.FromRgb(30, 30, 30);
        BackgroundSprite.Fill.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(targetColor, TimeSpan.FromSeconds(0.25)));
    }

    private void Toggle_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (ShouldToggle) OutputPort.Signal = !OutputPort.Signal;
        ChangeColor(OutputPort.Signal);

        Gate_MouseUp(sender, e);
    }
}
