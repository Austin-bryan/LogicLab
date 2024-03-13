using System.Windows;
using System.Windows.Input;

namespace LogicLab;

public partial class MainWindow : Window
{
    public List<LogicComponent> draggingComponents = [];
    private Point dragStart;
    private readonly ComponentDragger dragger;

    public MainWindow()
    {
        InitializeComponent();
        dragger = new(mainGrid);
    }

    public void DragStart(Point dragStart)
    {
        this.dragStart = dragStart;
    }
    public void DragEnd() 
    {
        draggingComponents.Clear();
    }
    private void Grid_MouseMove(object sender, MouseEventArgs e)
    {
        foreach (LogicComponent component in draggingComponents)
            component.Drag(e.GetPosition(component) - dragStart);

        dragger.MouseMove(e);
    }

    private void Grid_MouseUp(object sender, MouseButtonEventArgs e) => dragger.MouseUp(e);
    private void Grid_MouseDown(object sender, MouseButtonEventArgs e) => dragger.MouseDown(e);
}