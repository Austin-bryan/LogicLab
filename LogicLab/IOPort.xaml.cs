using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Input;

namespace LogicLab;

public enum EPortType { Input, Output }

public partial class IOPort : UserControl
{
    private EPortType portType;

    private readonly SolidColorBrush idleColor, hoverColor;
    private bool isPressed;

    private Wire wire;
    private static Wire? activeWire;


    public IOPort(EPortType portType)
    {
        InitializeComponent();

        idleColor     = new SolidColorBrush(Color.FromRgb(9, 180, 255));
        hoverColor    = new SolidColorBrush(Color.FromRgb(59, 230, 255));
        this.portType = portType;

        BitmapImage bitmapImage = new(new Uri($"Images/{this.portType}.png", UriKind.Relative));
        ImageBrush imageBrush = new(bitmapImage);

        Sprite.Fill = idleColor;
        Sprite.OpacityMask = imageBrush;

        if (portType == EPortType.Input)
            return;

        // You may need to adjust the hover size as per your requirements
        Sprite.Height *= 2.5;
        Sprite.Width *= 1.75;
    }
    private void Sprite_MouseEnter(object sender, MouseEventArgs e) => Sprite.Fill = hoverColor;
    private void Sprite_MouseLeave(object sender, MouseEventArgs e) => Sprite.Fill = idleColor;

    private void Wire_MouseMove(object sender, MouseEventArgs e)
    {
        if (!isPressed) 
            return;
        wire.Draw(portType, PointToScreen(e.GetPosition(this)));
    }
    private void Wire_MouseUp(object sender, MouseEventArgs e)
    {
        isPressed = false;
        this.MainGrid().MouseMove -= Wire_MouseMove;
        this.MainGrid().MouseUp -= Wire_MouseUp;
    }

    private void Sprite_MouseDown(object sender, MouseEventArgs e)
    {
        isPressed = true;
        this.MainGrid().MouseMove += Wire_MouseMove;
        this.MainGrid().MouseUp += Wire_MouseUp;

        wire = new Wire(this.MainWindow(), PointToScreen(
            new Point(
                Sprite.Margin.Left + (portType == EPortType.Input ? ActualWidth : 0),
                Sprite.Margin.Right + ActualHeight / 2)));
        activeWire = wire;
        wire.Draw(portType, PointToScreen(e.GetPosition(this)));
    }

    private void Sprite_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (activeWire == null)
            return;

        activeWire.SetEndPoint(portType, PointToScreen(
            new Point(
                Sprite.Margin.Left + (portType == EPortType.Input ? ActualWidth : 0), 
                Sprite.Margin.Right + ActualHeight / 2)));
        activeWire = null;

        if (portType == EPortType.Input)
            Sprite.Visibility = Visibility.Hidden;
    }

    private void Grid_Loaded(object sender, RoutedEventArgs e)
    {
        
    }
}