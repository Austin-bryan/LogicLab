using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Effects;
using System.Windows.Media;

namespace LogicLab.Component;

// Austin
public abstract class LabComponent : UserControl
{
    protected static bool ShiftKey => Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
    protected readonly DropShadowEffect shadow = new() { ShadowDepth = 4, Color = Colors.Black, Opacity = 0.8, BlurRadius = 6 };
    protected readonly DropShadowEffect highlight = new() { ShadowDepth = 0, Color = Color.FromRgb(200, 200, 255), Opacity = 1, BlurRadius = 10 };
    protected DropShadowEffect animatingShadow;

    // TODO:: Shouldnt this call InitComponent()?
    public LabComponent() => animatingShadow = shadow;
    public abstract void ShowSignal(bool? signal);
}
