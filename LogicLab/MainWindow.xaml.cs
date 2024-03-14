using System.Windows;
using System.Windows.Input;

namespace LogicLab;

public partial class MainWindow : Window
{
    public MainWindow() => InitializeComponent();

    private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)
            && e.OriginalSource == MainGrid)
            ComponentSelector.DeselectAll();
    }
}//50, 16