using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

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
        // Deslect if they tap on the background without the shift key
        // Checking for original source prevents this from being fired if the user clicks on a control within the grid,
        // only if they click on the grid directly
        if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)
            && e.OriginalSource == MainGrid)
            ComponentSelector.DeselectAll();
        if (e.OriginalSource == MainGrid)
            ComponentSelector.MouseDown(e);
    }

    private void MainGrid_MouseMove(object sender, MouseEventArgs e) => ComponentSelector.MouseMove(e);
    private void MainGrid_MouseUp(object sender, MouseButtonEventArgs e) => ComponentSelector.MouseUp(e);
    private void MainGrid_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        // TODO::Delete me
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (Keyboard.Modifiers != ModifierKeys.Shift)
            return;
        // Shortcuts for alignment of logic gates
        switch (e.Key)
        {
        case Key.A: ComponentSelector.AlignLeft();   break;
        case Key.W: ComponentSelector.AlignUp();     break;
        case Key.D: ComponentSelector.AlignRight();  break;
        case Key.S: ComponentSelector.AlignDown();   break;
        case Key.C: ComponentSelector.AlignCenter(); break;
        case Key.M: ComponentSelector.AlignMiddle(); break;
        default: break;
        }
    }

    private void MainGrid_KeyDown(object sender, KeyEventArgs e)
    {
        // TODO:: Delete me
    }
}//50, 16