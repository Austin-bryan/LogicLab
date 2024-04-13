using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LogicLab.Component;
using LogicLab.EditorUI;

namespace LogicLab;

public partial class MainWindow : Window
{
    private CreationMenu? creationMenu;

    private Point oldMousePos   = new(0, 0);//GA
    private bool hasPanned;

    public MainWindow()
    {
        InitializeComponent();
        ComponentSelector.MainGrid = MainGrid;
        WindowState = WindowState.Maximized;
        InitializeDotGrid();
    }
    private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
    {
        // Deslect if they tap on the background without the shift key
        // Checking for original source prevents this from being fired if the user clicks on a control within the grid,
        // only if they click on the grid directly
        if (e.LeftButton == MouseButtonState.Pressed) // GA
        {
            // AB
            if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)
                && e.OriginalSource == MainGrid)
                ComponentSelector.DeselectAll();
            if (e.OriginalSource == MainGrid)
                ComponentSelector.MouseDown(e);
            // end AB
            CloseCreationMenu(e.Source); // GA
        }
        // GA
        if (e.MiddleButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
            MainGrid.Children.ToList().OfType<LogicComponent>().ToList().ForEach(c => c.MovementMode(false));   
    }

    private void MainGrid_MouseMove(object sender, MouseEventArgs e)
    {
        // GA  : this covers the panning when you click middle mouse button
        Point mouseDelta = new(e.GetPosition(this).X - oldMousePos.X, e.GetPosition(this).Y - oldMousePos.Y); //calculates mouseDelta
        oldMousePos = new Point(e.GetPosition(this).X, e.GetPosition(this).Y);

        if (e.MiddleButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed) 
        {
            // AB This enables to pan with right click, because I can't use middle mouse on a laptop
            if (creationMenu != null)
                CloseCreationMenu();
            hasPanned = true; // AB
            DotGridSegment.TranslateGrid(mouseDelta);//moves whole grid

            foreach (var item in MainGrid.Children.ToList().OfType<UserControl>()) //looks through all gates on grid 
                item.Translate(mouseDelta); //translates them by the mouse delta
            foreach (var item in MainGrid.Children.ToList().OfType<LogicComponent>()) item.UpdateAllWires();

            foreach (var item in MainGrid.Children.ToList().OfType<DotGridSegment>())
            {
                item.Translate(new Point(-mouseDelta.X, -mouseDelta.Y));
                item.UpdateGridPos();
            }
        }
        // end GA 

        if (e.LeftButton == MouseButtonState.Pressed) //GA
            ComponentSelector.MouseMove(e); //AB
    }
    // GA
    private void InitializeDotGrid()
    {
        foreach (DotGridSegment item in MainGrid.Children.ToList().OfType<DotGridSegment>())
            MainGrid.Children.ToList().Remove(item); //removes all old DotGridSegments
        
        for (int y = 0; y < 5; y += 1) 
            for (int x = 0; x < 5; x += 1)
            {
                DotGridSegment dotGrid = new(new Point(x, y));
                MainGrid.Children.Add(dotGrid);
                Panel.SetZIndex(dotGrid, -10);
                dotGrid.UpdateGridPos();
            }
    }
    // end GA

    private void MainGrid_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Released) //GA : this makes it so the selection box does not interfear with any other mouse function
            ComponentSelector.MouseUp(e); //AB
        if (e.MiddleButton == MouseButtonState.Released)//GA
            foreach (var item in MainGrid.Children.ToList().OfType<LogicComponent>()) item.MovementMode(false);//GA
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
        case Key.K: Application.Current.Shutdown(); break;
        default: break;
        }
    }

    private void MainGrid_KeyDown(object sender, KeyEventArgs e)
    {
        // TODO:: Delete me
    }

    // AB
    private void MainGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        hasPanned = false;
        if (HintLabel != null && HintLabel.Visibility == Visibility.Visible)
            MainGrid.Children.Remove(HintLabel);
    }
    private void MainGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (!hasPanned && e.OriginalSource == MainGrid)
            OpenCreationMenu(e.GetPosition(this));

    }
    // end AB

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
    public void CloseCreationMenu(object? source = null)
    {
        if (source == null || source != creationMenu)
        {
            MainGrid.Children.Remove(creationMenu); //GA
            creationMenu = null; // AB
        }
    }
}//50, 16