using System.Windows;
using System.Windows.Controls;

namespace LogicLab;

// Gary
public partial class InputPixel
{
    protected override Grid ControlGrid => Grid;

    public InputPixel() => InitializeComponent();
    protected override void Grid_Loaded(object sender, RoutedEventArgs e)
    {
        base.Grid_Loaded(sender, e);
        OnLoaded();
    }
}
