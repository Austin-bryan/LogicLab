using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace LogicLab;

public partial class LogicComponent : UserControl
{
    private static bool ShiftKey => Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
    private bool IsSelected => ComponentSelector.IsSelected(this);
    
    private readonly DropShadowEffect shadow = new()
    {
        ShadowDepth = 6, Color = Colors.Black, Opacity = 0.5, BlurRadius = 5
    };
    private readonly DropShadowEffect highlight = new()
    {
        ShadowDepth = 0, Color = Color.FromArgb(255, 200, 200, 255), Opacity = 1, BlurRadius = 30
    };
    private readonly ComponentDragger dragger;
    private DropShadowEffect animatingShadow;

    public LogicComponent()
    {
        animatingShadow = shadow;
        dragger = new ComponentDragger(this);
        Deselect();
    }

    public void Select(bool shiftSelect)
    {
        if (IsSelected)
            return;
        BeginShadowAnimation(shadow, highlight);

        if (shiftSelect)
             ComponentSelector.ShiftSelect(this);
        else ComponentSelector.SingleSelect(this);
    }
    public void Deselect()
    {
        BeginShadowAnimation(highlight, shadow);
        ComponentSelector.Deselect(this);
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

    protected virtual void Grid_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (!IsSelected && ShiftKey)
            Select(shiftSelect: true);
        else if (!IsSelected)
        {
            ComponentSelector.DeselectAll(this);
            Select(shiftSelect: false);
        }

        dragger.DragStart(e);
    }
    protected virtual void Grid_MouseUp(object sender, MouseButtonEventArgs e) => dragger.DragEnd();
    protected virtual void Grid_MouseMove(object sender, MouseEventArgs e) => dragger.DragMove(e);
    protected virtual void Grid_Loaded(object sender, RoutedEventArgs e)
    {
        if (Window.GetWindow(this) is MainWindow mw)
            if (mw.MainGrid != null)
                mw.MainGrid.MouseMove += Grid_MouseMove;
    }
}//113, 83