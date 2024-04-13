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
    private static Point gridPosition = new(0, 0); //the position for the whole grid
    private static double imageSize = 1024;
    private static double gridSpeedMultiplyer = 1;
    private static double dotSpacing = 500; //keeps track of the # of pixels in between each dot
    private Point reletivePosition = new(3, 3); //keeps track of where this segment of the grid is in the whole grid

    public DotGridSegment() => InitializeComponent();
    public DotGridSegment(Point reletivePosition)
    {
        InitializeComponent();
        this.reletivePosition = reletivePosition;
    }

    public void UpdateGridPos()
    {
        //this.SetPosition(reletivePosition);
        this.SetLeft(Convert.ToDouble(gridPosition.X) % dotSpacing + (reletivePosition.X * imageSize));
        this.SetTop (Convert.ToDouble(gridPosition.Y) % dotSpacing + (reletivePosition.Y * imageSize));
        dotSpacing = imageSize / 16; //calculates the # of pixels in between each dot; there are 32 dots in each image 
    }
    public static void TranslateGrid(Point mouseDelta) => gridPosition = new Point
        ((gridPosition.X + mouseDelta.X * gridSpeedMultiplyer), (gridPosition.Y + mouseDelta.Y * gridSpeedMultiplyer));
}
//32 dots
