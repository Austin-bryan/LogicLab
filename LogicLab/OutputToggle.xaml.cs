using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Security.Policy;
using System.Windows.Media.Imaging;

namespace LogicLab;

public partial class OutputToggle
{
    protected override Grid ControlGrid => Grid;
    private bool ShouldToggle = false;

    public OutputToggle() : base() => InitializeComponent();
    protected override void Grid_Loaded(object sender, RoutedEventArgs e)
    {
        base.Grid_Loaded(sender, e);
        OnLoaded();

        BackgroundSprite.MouseDown += Toggle_MouseDown;
        BackgroundSprite.MouseMove += Component_MouseMove;
        BackgroundSprite.MouseUp   += Toggle_MouseUp;

        BitmapImage bitmapImage = new(new Uri($"Images/OnOff.png", UriKind.Relative));

        var Sprite = new Rectangle
        {
            Height = 25,
            Width  = 30,
            Fill   = new ImageBrush(bitmapImage)
        };
        RenderOptions.SetBitmapScalingMode(Sprite, BitmapScalingMode.HighQuality);

        Sprite.MouseDown += Toggle_MouseDown;
        Sprite.MouseMove += Component_MouseMove;
        Sprite.MouseUp   += Toggle_MouseUp;
        Sprite.Cursor = Cursors.Hand;

        Grid.Children.Add(Sprite);

        Grid.UpdateLayout();
    }

    private void Background_MouseDown(object sender, MouseButtonEventArgs e)
    {
        Component_MouseDown(sender, e);
        //TODO: make wire update position when moves
    }

    private void Toggle_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (ShouldToggle)
            return;
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
    private void Toggle_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (ShouldToggle)
            OutputPort.Signal = !OutputPort.Signal;

        Component_MouseUp(sender, e);
    }
}