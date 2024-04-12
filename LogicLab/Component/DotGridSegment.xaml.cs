using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using static System.Net.Mime.MediaTypeNames;

namespace LogicLab.Component;
//GA
public partial class DotGridSegment : UserControl
{
    private static Point gridPosition = new Point(0, 0); //the position for the whole grid
    private static double dotSpacing = 500;

    public DotGridSegment() => InitializeComponent();
    public DotGridSegment(Point ReletivePosition)
    {
        InitializeComponent();
        Margin = new Thickness(ActualWidth * ReletivePosition.X, ActualHeight * ReletivePosition.Y, 0, 0);
    }

    public void UpdateGridPos()
    {
        this.SetPosition(new Point(gridPosition.X % dotSpacing, gridPosition.Y % dotSpacing));
        dotSpacing = ActualWidth / 32 * 2; //calculates the # of pixels in between each dot; there are 32 dots in each image 
    }
    public static void TranslateGrid(Point mouseDelta) => gridPosition = new Point
        ((gridPosition.X + mouseDelta.X) % dotSpacing, (gridPosition.Y + mouseDelta.Y) % dotSpacing);
}
//32 dots
