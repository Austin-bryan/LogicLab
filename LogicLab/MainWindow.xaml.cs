using System.Windows;
using System.Windows.Input;

namespace LogicLab;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        ComponentSelector.Grid = MainGrid;
        WindowState = WindowState.Maximized;
    }
    private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)
            && e.OriginalSource == MainGrid)
            ComponentSelector.DeselectAll();
        if (e.OriginalSource == MainGrid)
            ComponentSelector.MouseDown(e);
    }

    private void MainGrid_MouseMove(object sender, MouseEventArgs e) => ComponentSelector.MouseMove(e);
    private void MainGrid_MouseUp(object sender, MouseButtonEventArgs e) => ComponentSelector.MouseUp(e);
}//50, 16