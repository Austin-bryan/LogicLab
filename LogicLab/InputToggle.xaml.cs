using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

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
