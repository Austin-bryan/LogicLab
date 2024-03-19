using System.Windows;
using System.Windows.Controls;

namespace LogicLab;

public static class ExtensionMethods
{
    public static MainWindow MainWindow(this UserControl userControl) => Window.GetWindow(userControl) as MainWindow ?? throw new NullReferenceException("Null ref");

    public static Label DebugLabel(this UserControl userControl) => userControl.MainWindow().DebugLabel;
    public static Grid MainGrid(this UserControl userControl) => userControl.MainWindow().MainGrid;

    public static void SetLeft   (this Control control, double x) => control.Margin = new Thickness(x, control.Margin.Top, control.Margin.Right, control.Margin.Bottom);
    public static void SetTop    (this Control control, double x) => control.Margin = new Thickness(control.Margin.Left, x, control.Margin.Right, control.Margin.Bottom);
    public static void SetRight  (this Control control, double x) => control.Margin = new Thickness(control.Margin.Left, control.Margin.Top, x, control.Margin.Bottom);
    public static void SetBottom (this Control control, double x) => control.Margin = new Thickness(control.Margin.Left, control.Margin.Top, control.Margin.Right, x);

    public static double GetLeft   (this Control control) => control.Margin.Left;
    public static double GetTop    (this Control control) => control.Margin.Top;
    public static double GetRight  (this Control control) => control.Margin.Right;
    public static double GetBottom (this Control control) => control.Margin.Bottom;

    public static void AddLeft   (this Control control, double x) => control.SetLeft(control.GetLeft() + x);
    public static void AddTop    (this Control control, double x) => control.SetTop(control.GetTop() + x);
    public static void AddRight  (this Control control, double x) => control.SetRight(control.GetRight() + x);
    public static void AddBottom (this Control control, double x) => control.SetBottom(control.GetBottom() + x);

    public static void SubLeft   (this Control control, double x) => control.AddLeft(-x);
    public static void SubTop    (this Control control, double x) => control.AddTop(-x);
    public static void SubRight  (this Control control, double x) => control.AddRight(-x);
    public static void SubBottom (this Control control, double x) => control.AddBottom(-x);

    public static U? TryGetValue<T, U>(this Dictionary<T, U> dict, T key) where T : notnull => dict.TryGetValue(key, out U? value) ? value : default;
}
