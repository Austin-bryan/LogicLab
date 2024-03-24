using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace LogicLab;
public partial class OutputPixel
{
    protected override Grid ComponentGrid => Grid;
    protected override Rectangle BackgroundRect => BackgroundSprite;

    public OutputPixel()
    {
        InitializeComponent();
    }
    protected override void Grid_Loaded(object sender, RoutedEventArgs e)
    {
        OnLoaded();
    }
}
