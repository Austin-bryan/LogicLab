using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace LogicLab;

public enum EPortType { Input, Output }

public partial class IOPort : UserControl
{
    public Point WireConnection => PointToScreen(new(
                Sprite.Margin.Left + (portType == EPortType.Input ? ActualWidth / 4 : 0),
                Sprite.Margin.Right + ActualHeight / 2));
    private EPortType portType;

    private readonly SolidColorBrush idleColor, hoverColor;
    private bool isPressed;

    private readonly List<Wire> wires = [];
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

        // Rescale output port
        Sprite.Height *= 2.5;
        Sprite.Width *= 1.75;

        OverlapDetector.Height *= 2.5;
        OverlapDetector.Width *= 1.75;
    }
    public void OnDrag(MouseEventArgs deleteMe)
    {
        // Redraw all wires
        wires.ForEach(w => w.SetEndPoint(portType, WireConnection));
    }

    private void Sprite_MouseEnter(object sender, MouseEventArgs e) => Sprite.Fill = hoverColor;
    private void Sprite_MouseLeave(object sender, MouseEventArgs e) => Sprite.Fill = idleColor;
    private void Sprite_MouseDown(object sender, MouseEventArgs e)
    {
        isPressed = true;

        this.MainGrid().MouseMove += Wire_MouseMove;
        this.MainGrid().MouseUp += Wire_MouseUp;

        // Create new wire, and mark this as a port
        Wire wire = new(this.MainWindow(), WireConnection);
        wire.SetPort(portType, this);

        // Active wire is the one currently being dragged
        activeWire = wire;
        activeWire.Draw(PointToScreen(e.GetPosition(this)));
        wires.Add(wire);

        ShowSprite(false);
    }
    private void Sprite_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (activeWire == null)
            return;

        // If the user releases their mouse on this port, and there's an active wire
        // connect that wire to this port, making a fully connected wire
        activeWire.SetEndPoint(portType, WireConnection);
        activeWire.SetPort(portType, this);
        wires.Add(activeWire);

        ShowSprite(false);
    }

    private void ShowSprite(bool visible)
    {
        if (portType != EPortType.Input)
            return;

        Sprite.BeginAnimation(OpacityProperty,
            new DoubleAnimation(fromValue: visible ? 0 : 1,
                                toValue: visible ? 1 : 0, 
                                duration: TimeSpan.FromSeconds(0.5)));
    }

    private void Wire_MouseMove(object sender, MouseEventArgs e)
    {
        if (!isPressed) 
            return;
        // Redraw if there user is dragging the logic gate, and theres no active wire
        activeWire?.Draw(PointToScreen(e.GetPosition(this)));
    }
    private void Wire_MouseUp(object sender, MouseEventArgs e)
    {
        isPressed = false;
        this.MainGrid().MouseMove -= Wire_MouseMove;
        this.MainGrid().MouseUp -= Wire_MouseUp;

        bool wireIsHalfConneceted = activeWire?.ConnectedPorts.ContainsKey(portType == EPortType.Output ? EPortType.Input : EPortType.Output) == false;
        if (wireIsHalfConneceted)
        {
            activeWire?.Remove();
            wires.Remove(activeWire);
            activeWire = null;
            ShowSprite(true);
        }
    }
    private void Grid_Loaded(object sender, RoutedEventArgs e)
    {
        if (portType == EPortType.Output)
        {
            Margin = new Thickness(ActualWidth - ActualWidth / 2.8, 0, 0, 0);
            Sprite.Margin = new Thickness(OverlapDetector.Width / 6, OverlapDetector.Height / 6, 0, 0);
        }
    }
}