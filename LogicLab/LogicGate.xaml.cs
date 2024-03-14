using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LogicLab;

public partial class LogicGate : LogicComponent
{
    private ELogicGate gateType;


    private static List<ELogicGate> deleteMe;
    private static int count = 0;

    public LogicGate()
    {
        InitializeComponent();
        deleteMe = [ELogicGate.Buffer,
                    ELogicGate.AND,
                    ELogicGate.OR,
                    ELogicGate.XOR,
                    ELogicGate.NAND,
                    ELogicGate.NOR,
                    ELogicGate.XNOR];

        gateType = deleteMe[count];
        count++;

        ShowImage();
    }

    protected override void Grid_Loaded(object sender, System.Windows.RoutedEventArgs e)                => base.Grid_Loaded(sender, e);
    protected override void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)  => base.Grid_MouseDown(sender, e);
    protected override void Grid_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)        => base.Grid_MouseMove(sender, e);
    protected override void Grid_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)    => base.Grid_MouseUp(sender, e);

    private void ShowImage()
    {
        BitmapImage bitmapImage = new(new Uri($"Images/{gateType} Gate.png", UriKind.Relative));
        ImageBrush imageBrush = new(bitmapImage);

        GateImage.Fill = imageBrush;
    }
}
