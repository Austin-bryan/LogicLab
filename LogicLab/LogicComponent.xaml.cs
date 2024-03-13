using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace LogicLab;

public partial class LogicComponent : UserControl
{
    private readonly DropShadowEffect shadow = new()
    {
        ShadowDepth = 6, Color = Colors.Black, Opacity = 0.5, BlurRadius = 5
    };
    private readonly DropShadowEffect highlight = new()
    {
        ShadowDepth = 0, Color = Color.FromArgb(255, 200, 200, 255), Opacity = 1, BlurRadius = 30
    };

    private DropShadowEffect animatingShadow;

    public LogicComponent() => InitializeComponent();

    private void MainImage_MouseDown(object sender, MouseButtonEventArgs e)
    {
        var mw = Window.GetWindow(this) as MainWindow;

        mw.draggingComponents = [this];
        mw.DragStart(e.GetPosition(this));

        BeginShadowAnimation(shadow, highlight);
    }
    public void Drag(Vector delta)
    {
        Margin = new(Margin.Left + delta.X, Margin.Top + delta.Y, Margin.Right, Margin.Bottom);
    }
    private void MainImage_MouseUp(object sender, MouseButtonEventArgs e)
    {
        var mw = Window.GetWindow(this) as MainWindow;
        mw.DragEnd();

        BeginShadowAnimation(highlight, shadow);
    }

    private void BeginShadowAnimation(DropShadowEffect fromShadow, DropShadowEffect targetShadow)
    {
        TimeSpan animTime = TimeSpan.FromSeconds(0.125);

        animatingShadow = fromShadow.Clone();
        animatingShadow.BeginAnimation(DropShadowEffect.ShadowDepthProperty, new DoubleAnimation(targetShadow.ShadowDepth, animTime));
        animatingShadow.BeginAnimation(DropShadowEffect.ColorProperty,       new ColorAnimation (targetShadow.Color, animTime));
        animatingShadow.BeginAnimation(DropShadowEffect.OpacityProperty,     new DoubleAnimation(targetShadow.Opacity,animTime));
        animatingShadow.BeginAnimation(DropShadowEffect.BlurRadiusProperty,  new DoubleAnimation(targetShadow.BlurRadius, animTime));
        Effect = animatingShadow;
    }
}
