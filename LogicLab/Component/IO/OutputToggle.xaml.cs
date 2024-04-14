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
    protected override Rectangle ForegroundSprite => sprite;
    private bool ShouldToggle = false;
    private readonly Rectangle sprite;

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

        sprite.MouseLeftButtonDown += Toggle_MouseDown;
        sprite.MouseLeftButtonUp   += Toggle_MouseUp;
        sprite.MouseMove += Component_MouseMove;
        sprite.Cursor     = Cursors.Hand;
    }

    // AB
    protected override void Grid_Loaded(object sender, RoutedEventArgs e)
    {
        base.Grid_Loaded(sender, e);
        OnLoaded();

        BackgroundSprite.MouseLeftButtonDown += Toggle_MouseDown;
        BackgroundSprite.MouseLeftButtonUp   += Toggle_MouseUp;
        BackgroundSprite.MouseMove += Component_MouseMove;
        BackgroundSprite.Cursor     = Cursors.Hand;

        Grid.Children.Add(sprite);
        Grid.UpdateLayout();
    }
    
    private void Dragger_OnDragEnded(object? sender, EventArgs e) => sprite.Cursor = Cursors.Hand;
    private void Dragger_OnDragStarted(object? sender, EventArgs e) => (sprite.Cursor, ShouldToggle) = (Cursors.SizeAll, false);
    private void Background_MouseDown(object sender, MouseButtonEventArgs e) => Component_MouseLeftButtonDown(sender, e);

    // Gary
    private void Toggle_MouseDown(object sender, MouseButtonEventArgs e)
    {
        ShouldToggle = true;
        Component_MouseLeftButtonDown(sender, e);
    }
    private void Toggle_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (ShouldToggle)
            OutputPort?.SetSignal(!OutputPort.GetSignal(), []);

        Component_MouseLeftButtonUp(sender, e);
    }
}