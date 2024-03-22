using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace LogicLab;

public partial class InputToggle
{
    protected override Grid ComponentGrid => Grid;
    protected override Rectangle BackgroundRect => BackgroundSprite;

    public InputToggle()
    {
        InitializeComponent();
    }
    protected override void Grid_Loaded(object sender, RoutedEventArgs e)
    {
        OnLoaded();
        OutputPort.Signal = false;
    }

    private void Gate_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        //TODO: make this work and make it draggable
        OutputPort.Signal = !OutputPort.Signal;
    }
}
