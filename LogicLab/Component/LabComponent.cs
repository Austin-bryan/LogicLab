using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace LogicLab.Component;

// Austin
// Purpose: This is anything that can exist on the page, a wire, logic component,
// and other classes such as comment that were never finish
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
