using System.Windows;
using System.Windows.Controls;

namespace LogicLab;

// Gary
// Purpose: Just disables black orange or red depending if its off, on, or in an error state.
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