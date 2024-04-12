using System.Windows;
using System.Windows.Controls;

namespace LogicLab;

// Austin
public static class ExtensionMethods
{
    public static MainWindow MainWindow(this UserControl userControl) => Window.GetWindow(userControl) as MainWindow ?? throw new NullReferenceException("Null ref");
    public static Label DebugLabel(this UserControl userControl) => userControl.MainWindow().DebugLabel;
    public static Grid MainGrid(this UserControl userControl) => userControl.MainWindow().MainGrid;

    public static List<UIElement> ToList(this UIElementCollection uec)
    {
        List<UIElement> list = [];

        foreach (UIElement element in uec)
            list.Add(element);
        return list;
    }

    public static void SetLeft   (this FrameworkElement control, double x) => control.Margin = new Thickness(x, control.Margin.Top, control.Margin.Right, control.Margin.Bottom);
    public static void SetTop    (this FrameworkElement control, double x) => control.Margin = new Thickness(control.Margin.Left, x, control.Margin.Right, control.Margin.Bottom);
    public static void SetRight  (this FrameworkElement control, double x) => control.Margin = new Thickness(control.Margin.Left, control.Margin.Top, x, control.Margin.Bottom);
    public static void SetBottom (this FrameworkElement control, double x) => control.Margin = new Thickness(control.Margin.Left, control.Margin.Top, control.Margin.Right, x);

    public static double GetLeft   (this FrameworkElement control) => control.Margin.Left;
    public static double GetTop    (this FrameworkElement control) => control.Margin.Top;
    public static double GetRight  (this FrameworkElement control) => control.Margin.Right;
    public static double GetBottom (this FrameworkElement control) => control.Margin.Bottom;

    public static void AddLeft   (this FrameworkElement control, double x) => control.SetLeft(control.GetLeft() + x);
    public static void AddTop    (this FrameworkElement control, double x) => control.SetTop(control.GetTop() + x);
    public static void AddRight  (this FrameworkElement control, double x) => control.SetRight(control.GetRight() + x);
    public static void AddBottom (this FrameworkElement control, double x) => control.SetBottom(control.GetBottom() + x);

    public static void SubLeft   (this FrameworkElement control, double x) => control.AddLeft(-x);
    public static void SubTop    (this FrameworkElement control, double x) => control.AddTop(-x);
    public static void SubRight  (this FrameworkElement control, double x) => control.AddRight(-x);
    public static void SubBottom (this FrameworkElement control, double x) => control.AddBottom(-x);

    public static void SetPosition(this FrameworkElement control, Point p) => control.Margin = new Thickness(p.X, p.Y, 0, 0);
    public static void Translate(this  FrameworkElement control, double x, double y) => control.SetPosition(new Point(control.GetLeft() + x, control.GetTop() + y)); //GA: takes 2 inputs and moves control by that amount

    public static void Show(this object obj) => MessageBox.Show(obj.ToString());

    public static U? TryGetValue<T, U>(this Dictionary<T, U> dict, T key) where T : notnull => dict.TryGetValue(key, out U? value) ? value : default;
}
