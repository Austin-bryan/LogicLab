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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LogicLab;
public partial class InputConstant
{
    protected override Grid ComponentGrid => Grid;
    protected override Rectangle BackgroundRect => BackgroundSprite;
    private bool? OutValue = null;

    public InputConstant()
    {
        InitializeComponent();
    }
    protected override void Grid_Loaded(object sender, RoutedEventArgs e)
    {
        OnLoaded();

        Random random = new();
        OutValue = false;
        if (random.Next(0, 2) == 1) OutValue = true; //temporary constent randomization

        OutputPort.Signal = OutValue;
        ChangeColor(OutValue);
    }
    private void ChangeColor(bool? signal)
    {
        Color targetColor = signal == true ? Color.FromRgb(150, 150, 30) : Color.FromRgb(30, 30, 30);
        BackgroundSprite.Fill.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(targetColor, TimeSpan.FromSeconds(0.25)));
    }

}
