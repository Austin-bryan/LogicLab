using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using LogicLab.Component;
using LogicLab.EditorUI;

namespace LogicLab;

public partial class MainWindow : Window
{
    private CreationMenu? creationMenu;

    private bool mouseDown = false;//GA
    private double OldMouseX = 0, OldMouseY = 0;//GA

    public MainWindow()
    {
        InitializeComponent();
        ComponentSelector.MainGrid = MainGrid;
        WindowState = WindowState.Maximized;
    }
    private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
    {
        // Deslect if they tap on the background without the shift key
        // Checking for original source prevents this from being fired if the user clicks on a control within the grid,
        // only if they click on the grid directly
        if (e.LeftButton == MouseButtonState.Pressed) // << GA
        {
            // AB
            if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)
                && e.OriginalSource == MainGrid)
                ComponentSelector.DeselectAll();
            if (e.OriginalSource == MainGrid)
                ComponentSelector.MouseDown(e);
            // /AB
            CloseCreationMenu(e); //GA
        }
        if (e.MiddleButton == MouseButtonState.Pressed)
        {
            mouseDown = true; //GA
            CloseCreationMenu(e); //GA
        }
    }

    private void MainGrid_MouseMove(object sender, MouseEventArgs e)
    {
        //vv GA vv
        double mouseDeltaX = OldMouseX - e.GetPosition(this).X, mouseDeltaY = OldMouseY - e.GetPosition(this).Y;
        OldMouseX = e.GetPosition(this).X;
        OldMouseY = e.GetPosition(this).Y;
        if (mouseDown && e.MiddleButton == MouseButtonState.Pressed)
        {
            foreach (var item in MainGrid.Children.ToList().OfType<UserControl>())//moves all gates on grid
            {
                item.SubLeft(mouseDeltaX);
                item.SubTop(mouseDeltaY);
            }
        }
        //^^ GA ^^
        if (e.LeftButton == MouseButtonState.Pressed) /*<<GA*/ ComponentSelector.MouseMove(e); /*<<Astin*/
    }


    private void MainGrid_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Released) /*<<GA*/ 
            ComponentSelector.MouseUp(e); /*<<Astin*/
        if (e.MiddleButton == MouseButtonState.Released) 
            mouseDown = false; //GA
    }

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
        if (e.OriginalSource == MainGrid)
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
    public void CloseCreationMenu(MouseButtonEventArgs e)
    {
        if (e.Source != creationMenu)
            MainGrid.Children.Remove(creationMenu); //GA
    }
}//50, 16