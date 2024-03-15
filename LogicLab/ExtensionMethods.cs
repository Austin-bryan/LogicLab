using System.Windows;
using System.Windows.Controls;

namespace LogicLab;

public static class ExtensionMethods
{
    public static MainWindow MainWindow(this UserControl userControl)
    {
        return Window.GetWindow(userControl) as MainWindow ?? throw new NullReferenceException("Null ref");
    }

    public static Label DebugLabel(this UserControl userControl) => userControl.MainWindow().DebugLabel;
    public static Grid MainGrid(this UserControl userControl) => userControl.MainWindow().MainGrid;
}
