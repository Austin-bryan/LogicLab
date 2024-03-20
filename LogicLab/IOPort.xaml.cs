using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Collections.Immutable;

namespace LogicLab;

public enum EPortType { Input, Output }

public partial class IOPort : UserControl
{
    public Point WireConnection => PointToScreen(new(
                Sprite.Margin.Left + (portType == EPortType.Input ? ActualWidth / 2 : 0),
                Sprite.Margin.Right + ActualHeight / 2));

    public bool? Signal
    {
        get => _signal;
        set
        {
            _signal = value;

            (owningComponent as LogicGate).BackgroundSprite.Fill = 
                value == true ? new SolidColorBrush(Color.FromRgb(150, 150, 30))
                              : new SolidColorBrush(Color.FromRgb(30, 30, 30));

            if (portType == EPortType.Output)
            {
                foreach (var port in ConnectedPorts)
                    if (port.portType == EPortType.Input)
                        port.Signal = value;

                foreach (var wire in wires)
                {
                    if (wire.Output == this)
                        wire.Draw(new(0, 0), value);
                }
            }
            else owningComponent.OnInputChange(this);
        }
    }
    public ImmutableList<IOPort> ConnectedPorts
    {
        get
        {
            List<IOPort> connectedPorts = [];
            foreach (var wire in wires)
                foreach (var port in wire.ConnectedPorts.Values)
                    connectedPorts.Add(port);
            return [.. connectedPorts];
        }
    }

    private EPortType portType;
    private readonly LogicComponent owningComponent;
    private readonly SolidColorBrush idleColor, hoverColor;
    private bool isPressed;

    private readonly List<Wire> wires = [];
    private static Wire? activeWire;
    private bool? _signal = null;

    public IOPort(EPortType portType, LogicComponent owningComponent)
    {
        InitializeComponent();

        this.owningComponent = owningComponent;
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
        // TODO:: This causes the redraw for both output and input ports, when only one is needed
        // What needs to happen is the port should find out if the port being dragged is input or output
        // From there, only that port type is allowed to redraw
        wires.ForEach(w => w.Draw(WireConnection, w.Output?.Signal));
    }

    private void Sprite_MouseEnter(object sender, MouseEventArgs e) => Sprite.Fill = hoverColor;
    private void Sprite_MouseLeave(object sender, MouseEventArgs e) => Sprite.Fill = idleColor;
    private void Sprite_MouseDown(object sender, MouseEventArgs e)
    {
        isPressed = true;

        this.MainGrid().MouseMove += Wire_MouseMove;
        this.MainGrid().MouseUp += Wire_MouseUp;

        ShowSprite(false);

        // Create new wire, and mark this as a port
        Wire wire = new(this.MainWindow(), WireConnection);
        wire.SetPort(portType, this);

        // Active wire is the one currently being dragged
        activeWire = wire;
        activeWire.Draw(PointToScreen(e.GetPosition(this)), Signal);

        if (!wires.Contains(wire))
            wires.Add(wire);
    }
    private void Sprite_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (activeWire == null)
            return;

        ShowSprite(false);

        // If the user releases their mouse on this port, and there's an active wire
        // connect that wire to this port, making a fully connected wire
        activeWire.SetPort(portType, this);
        wires.Add(activeWire);

        if (activeWire.Output != null)
        {
            Signal = activeWire.Output.Signal;
            //MessageBox.Show(Signal.ToString());
        }
        owningComponent.OnInputChange(this);
        activeWire.Draw(WireConnection, Signal);

    }

    private void ShowSprite(bool visible)
    {
        if (portType != EPortType.Input)
            return;

        if (wires.Count == 0)
            Sprite.BeginAnimation(OpacityProperty,
                new DoubleAnimation(fromValue: visible ? 0 : 1,
                                    toValue: visible ? 1 : 0, 
                                    duration: TimeSpan.FromSeconds(0.5)));
    }

    private void Wire_MouseMove(object sender, MouseEventArgs e)
    {
        if (!isPressed) 
            return;
        // Redraw if there user is dragging the logic gate, and theres an active wire
        activeWire?.Draw(PointToScreen(e.GetPosition(this)), Signal);
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
        this.MainWindow().DebugLabel.Content = "";

        if (portType == EPortType.Output)
        {
            //Margin = new Thickness(ActualWidth - ActualWidth / 3, 0, 0, 0);
            Sprite.Margin = new Thickness(OverlapDetector.Width / 6, OverlapDetector.Height / 6, 0, 0);
        }
    }
}