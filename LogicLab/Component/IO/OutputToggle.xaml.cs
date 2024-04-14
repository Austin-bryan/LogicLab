using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace LogicLab;

// Gary
public partial class OutputToggle
{
    protected override Grid ControlGrid => Grid;
    private bool ShouldToggle = false;
    private readonly Rectangle sprite;
    protected override Rectangle ForegroundSprite => sprite;

    public OutputToggle() : base()
    {
        InitializeComponent();
        Dragger.OnDragStarted += Dragger_OnDragStarted;
        Dragger.OnDragEnded   += Dragger_OnDragEnded;

        sprite = new Rectangle
        {
            Height = 25,
            Width  = 30,
            Fill   = Utilities.GetImage("OnOff")
        };
        RenderOptions.SetBitmapScalingMode(sprite, BitmapScalingMode.HighQuality);

        sprite.MouseDown += Toggle_MouseDown;
        sprite.MouseMove += Component_MouseMove;
        sprite.MouseUp   += Toggle_MouseUp;
        sprite.Cursor     = Cursors.Hand;
    }

    // AB
    private void Dragger_OnDragEnded(object? sender, EventArgs e) => sprite.Cursor = Cursors.Hand;
    private void Dragger_OnDragStarted(object? sender, EventArgs e) => (sprite.Cursor, ShouldToggle) = (Cursors.SizeAll, false);

    // Austin
    protected override void Grid_Loaded(object sender, RoutedEventArgs e)
    {
        base.Grid_Loaded(sender, e);
        OnLoaded();

        BackgroundSprite.MouseDown += Toggle_MouseDown;
        BackgroundSprite.MouseMove += Component_MouseMove;
        BackgroundSprite.MouseUp   += Toggle_MouseUp;

        Grid.Children.Add(sprite);
        Grid.UpdateLayout();
    }

    private void Background_MouseDown(object sender, MouseButtonEventArgs e) => Component_MouseDown(sender, e);

    // Gary
    private void Toggle_MouseDown(object sender, MouseButtonEventArgs e)
    {
        ShouldToggle = true;
        Component_MouseDown(sender, e);
    }
    // Gary
    private void Toggle_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (ShouldToggle)
            OutputPort.SetSignal(!OutputPort.GetSignal(), []);

        Component_MouseUp(sender, e);
    }
}