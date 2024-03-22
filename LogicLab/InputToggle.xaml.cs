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
    }
}
