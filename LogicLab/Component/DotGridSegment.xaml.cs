using System.Windows;
using System.Windows.Controls;
namespace LogicLab.Component;

//GA
public partial class DotGridSegment : UserControl
{
    private static Point gridPosition        = new(0, 0); //the position for the whole grid
    private static double dotSpacing         = 500; //keeps track of the # of pixels in between each dot
    private const double imageSize           = 1024;
    private const double gridSpeedMultiplyer = 1;
    private Point reletivePosition           = new(3, 3); //keeps track of where this segment of the grid is in the whole grid

    public DotGridSegment() => InitializeComponent();
    public DotGridSegment(Point reletivePosition)
    {
        InitializeComponent();
        this.reletivePosition = reletivePosition;
    }

    public void UpdateGridPos()
    {
        this.SetLeft(Convert.ToDouble(gridPosition.X) % dotSpacing + (reletivePosition.X * imageSize));
        this.SetTop (Convert.ToDouble(gridPosition.Y) % dotSpacing + (reletivePosition.Y * imageSize));
        dotSpacing = imageSize / 16; //calculates the # of pixels in between each dot; there are 32 dots in each image 
    }
    public static void TranslateGrid(Point mouseDelta) => gridPosition = new Point
        ((gridPosition.X + mouseDelta.X * gridSpeedMultiplyer), (gridPosition.Y + mouseDelta.Y * gridSpeedMultiplyer));
}
//32 dots