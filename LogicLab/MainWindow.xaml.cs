using System.Windows;
using System.Windows.Input;
using LogicLab.Component;
using LogicLab.EditorUI;

namespace LogicLab;

public partial class MainWindow : Window
{
    private CreationMenu? creationMenu;
    public static bool InCommentMode { get; private set;  }

    public MainWindow()
    {
        InitializeComponent();
        ComponentSelector.MainGrid = MainGrid;
        WindowState = WindowState.Maximized;
    }

    private void OpenComment(object sender, RoutedEventArgs e)
    {
        Cursor = InCommentMode ? Cursors.Cross : Cursors.Arrow;
        InCommentMode = !InCommentMode;

        "Test".Show();
        Comment comment = new();

        MainGrid.Children.Insert(0, comment);
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
        case Key.W: ComponentSelector.AlignTop();    break;
        case Key.D: ComponentSelector.AlignRight();  break;
        case Key.S: ComponentSelector.AlignBottom(); break;
        case Key.C: ComponentSelector.AlignCenter(); break;
        case Key.M: ComponentSelector.AlignMiddle(); break;
        default: break;
        }
    }

    private void MainGrid_KeyDown(object sender, KeyEventArgs e)
    {
        // TODO:: Delete me
    }

    private void MainGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.OriginalSource != MainGrid)
            return;
        OpenCreationMenu(e.GetPosition(this));
    }

    public void OpenCreationMenu(Point point, EPortType? portType = null)
    {
        if (creationMenu != null)
            MainGrid.Children.Remove(creationMenu);

        creationMenu = new CreationMenu();
        MainGrid.Children.Add(creationMenu);
        creationMenu.SetPosition(point);

        if (portType == null)
            return;

        if (portType == EPortType.Input)
             creationMenu.HideInput();
        else creationMenu.HideOutput();
    }
}//50, 16