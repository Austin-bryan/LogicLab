using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace LogicLab.EditorUI;

// Austin (entire class)
public partial class CreationItem: UserControl
{
    private readonly SolidColorBrush transparentBrush = new(Color.FromArgb(0, 0, 0, 0));
    private readonly SolidColorBrush highlightBrush = new(Color.FromArgb(255, 40, 40, 40));
    private readonly Func<LogicComponent> BuildLogicComponent;
    private readonly CreationMenu creationMenu;
    private readonly MainWindow mainWindow;

    public CreationItem(CreationMenu creationMenu, MainWindow mainWindow, string label, 
        Func<LogicComponent> buildLogicComponent, ImageBrush? image = null, bool showNot = false)
    {
        InitializeComponent();

        BuildLogicComponent = buildLogicComponent;
        this.creationMenu   = creationMenu;
        this.mainWindow     = mainWindow; 
        ItemLabel.Content   = label;

        ItemLabel.MouseEnter += Highlight_MouseEnter;
        ItemLabel.MouseLeave += Highlight_MouseLeave;
        ItemLabel.MouseDown  += Highlight_MouseDown;

        if (image != null)
            Graphic.OpacityMask = image;
        if (showNot)
            Not.Visibility = System.Windows.Visibility.Visible;
    }

    private void Highlight_MouseEnter(object sender, MouseEventArgs e) => Highlight.Fill = highlightBrush;
    private void Highlight_MouseLeave(object sender, MouseEventArgs e) => Highlight.Fill = transparentBrush;

    private void Highlight_MouseDown(object sender, MouseButtonEventArgs e)
    {
        creationMenu.Remove();
        LogicComponent logicComponent = BuildLogicComponent();

        mainWindow.MainGrid.Children.Add(logicComponent);

        Point mousePos = Mouse.GetPosition(mainWindow); 

        logicComponent.SetPosition(new Point(mousePos.X + 150, mousePos.Y));
        logicComponent.Select(false);
    }
}
