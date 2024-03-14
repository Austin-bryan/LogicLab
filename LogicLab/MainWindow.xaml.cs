using System.Windows;
using System.Windows.Input;

namespace LogicLab;

public partial class MainWindow : Window
{
    public List<LogicComponent> draggingComponents = [];
    private bool isMouseDown;
    private Point dragStart;
    private readonly ComponentDragger dragger;

    public MainWindow()
    {
        InitializeComponent();
        //dragger = new();
    }

    public void DragStart(Point dragStart)
    {
        //isMouseDown = true;
        //this.dragStart = dragStart;
    }
    public void DragEnd() 
    {
        //isMouseDown = false;
        //draggingComponents.Clear();
    }
    private void Grid_MouseMove(object sender, MouseEventArgs e)
    {
        //if (!isMouseDown)
        //    return;
        //foreach (LogicComponent component in ComponentSelector.SelectedComponents)
        //    component.Drag(e.GetPosition(component) - dragStart);

        //dragger.MouseMove(e);
    }

    private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
    {
        //dragger.MouseUp(e);
    }

    private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
    {
        //dragger.MouseDown(e);

        if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)
            && e.OriginalSource == MainGrid)
            ComponentSelector.DeselectAll();
    }
}