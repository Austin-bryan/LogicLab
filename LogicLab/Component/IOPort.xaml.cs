using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.Immutable;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using LogicLab.Component;
using System.Diagnostics;

namespace LogicLab;

public enum EPortType { Input, Output }

// Austin (entire class)     
public  partial class IOPort : UserControl
{
    public Point WireConnection => PointToScreen(new(
                Sprite.Margin.Left + (portType == EPortType.Input ? + (Parent is StackPanel _ ? ActualWidth / 2 : ActualWidth / 4) : 0),
                Sprite.Margin.Right + ActualHeight / 2));
    private static int count;
    private int ID;

    public bool? GetSignal() => _signal;

    private int debug = 0;

    private static int loopbreak = 0;

    public async void SetSignal(bool? value, List<SignalPath> propagationHistory)
    {
        //if (propagationHistory.Count == 0)
        //    owningComponent.MainWindow().DebugLabel.Content += "\n";
        //owningComponent.MainWindow().DebugLabel.Content += " :" + propagationHistory.Count.ToString() + ", " + owningComponent.id + ": ";
        //owningComponent.MainWindow().DebugLabel.Content += "(" + owningComponent.ID + ")";

        StackTrace stackTrace = new(); // get call stack
        StackFrame[] stackFrames = stackTrace.GetFrames();  // get method calls (frames)

        // write call stack method names
        //foreach (StackFrame stackFrame in stackFrames)
        //{
            //owningComponent.MainWindow().DebugLabel.Content += stackFrame.GetMethod().Name.ToString() + " ";
        //}
        //owningComponent.MainWindow().DebugLabel.Content += "\n";

        
        owningComponent.MainWindow().DebugLabel.Content += "ID: " + ID + ", ";
        
        if (value == _signal) 
        {
            return;
        }

        SignalPath currentPath = new();
        if (propagationHistory.Count != 0)
        {
            // Copy the last path to continue from there
            var lastPath = propagationHistory.Last();
            currentPath.AddRange(lastPath);
        }
        currentPath.AddStep(this, value);

        //owningComponent.MainWindow().DebugLabel.Content += "\n";
        foreach (var path in propagationHistory)
        {
            if (currentPath.Equals(path))
            {
                owningComponent.MainWindow().DebugLabel.Content += "Loop detected: " + ID + ", ";
                return;
            }
        }


        //if (portType == EPortType.Input && propagationHistory.Contains((this, value)))
        {
            //owningComponent.MainWindow().DebugLabel.Content += "Returned: " + ID + ", ";
            //"Exiting".Show();
            //owningComponent.MainWindow().DebugLabel.Content += "e";
            //await Task.Delay(100);
            //return;
        }
        if (loopbreak++ > 1000)
            return;

        propagationHistory.Add(currentPath);  
        _signal = value;
        owningComponent.ShowSignal(value);
        ProcessSignalAsync(value, propagationHistory);
    }
    public bool Connectionless => wires.Count == 0;

    private async void ProcessSignalAsync(bool? signal, List<SignalPath> propagationHistory)
    {
        if (portType == EPortType.Input)
            owningComponent.OnInputChange(this, propagationHistory);        
        else  // If Output
        {
            //await Task.Delay(100);
            foreach (var port in ConnectedPorts)
            {
                if (port.portType == EPortType.Input)
                {
                    //owningComponent.MainWindow().DebugLabel.Content += propagationHistory.Count.ToString();
                    port.SetSignal(signal, propagationHistory);
                }
            }

            foreach (var wire in wires)
                if (wire.Output == this)
                    wire.ShowSignal(signal);
        }
    }
    private void ShowSignal(bool? signal) => owningComponent.ShowSignal(signal);

    private void UpdateBackground(bool? signal)
    {
        if (owningComponent is LogicGate logicGate)
            logicGate.BackgroundSprite.Fill =
                signal == true ? new SolidColorBrush(Color.FromRgb(150, 150, 30))
                               : new SolidColorBrush(Color.FromRgb(30, 30, 30));
    }

    public ImmutableList<IOPort> ConnectedPorts
    {
        get
        {
            List<IOPort> connectedPorts = [];
            foreach (var wire in wires)
                foreach (var port in wire.ConnectedPorts.Values)
                    if (port != this && !connectedPorts.Contains(port))
                        if (portType == EPortType.Input  && port.portType == EPortType.Output ||
                            portType == EPortType.Output && port.portType == EPortType.Input)
                                connectedPorts.Add(port);
            //owningComponent.MainWindow().DebugLabel.Content += "C{" + connectedPorts.Count + "}";

            return [.. connectedPorts];
        }
    }
    public void RemoveWire(Wire wire) => wires.Remove(wire);

    private readonly EPortType portType;
    private readonly LogicComponent owningComponent;
    private readonly SolidColorBrush idleColor, hoverColor;
    private bool isPressed;

    private readonly List<Wire> wires = [];
    private static Wire? activeWire;
    private bool? _signal = null;

    public IOPort(EPortType portType, LogicComponent owningComponent)
    {
        InitializeComponent();
        ID = count++;
        IDLabel.Content = ID;

        this.owningComponent = owningComponent;
        idleColor            = new SolidColorBrush(Color.FromRgb(9, 180, 255));
        hoverColor           = new SolidColorBrush(Color.FromRgb(59, 230, 255));
        this.portType        = portType;

        BitmapImage bitmapImage = new(new Uri($"Images/{this.portType}.png", UriKind.Relative));
        ImageBrush imageBrush   = new(bitmapImage);

        Sprite.Fill        = idleColor;
        Sprite.OpacityMask = imageBrush;

        if (portType == EPortType.Input)
            return;

        // Rescale output port
        Sprite.Height *= 2.5;
        Sprite.Width *= 1.75;

        OverlapDetector.Height *= 2.5;
        OverlapDetector.Width *= 1.75;
    }
    public async void OnDrag()
    {
        // TODO:: This causes the redraw for both output and input ports, when only one is needed
        // What needs to happen is the port should find out if the port being dragged is input or output
        // From there, only that port type is allowed to redraw
        
        await Task.Delay(1);    
        wires.ForEach(w => w.Draw(WireConnection, w.Output?.GetSignal()));
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
        activeWire.Draw(PointToScreen(e.GetPosition(this)), GetSignal());

        if (!wires.Contains(wire))
            wires.Add(wire);
    }
    private void Sprite_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (activeWire == null)
            return;
        if (portType == EPortType.Output && activeWire.Output != null)
            return;
        if (portType == EPortType.Input && activeWire.Input != null)
            return;

        ShowSprite(false);

        if (portType == EPortType.Input)
            RemoveWires(this);
        else   
        {
            RemoveWires(activeWire.Input);
            activeWire.Input.wires.Add(activeWire);
        }

        // If the user releases their mouse on this port, and there's an active wire
        // connect that wire to this port, making a fully connected wire
        activeWire.SetPort(portType, this);
        wires.Add(activeWire);

        if (activeWire.Output != null)
        {
            owningComponent.MainWindow().DebugLabel.Content += "\n";
            SetSignal(activeWire.Output.GetSignal(), []);
        }
        //activeWire.Input?.SetSignal(GetSignal(), []);

        //owningComponent.OnInputChange(this);
        activeWire.Draw(WireConnection, GetSignal());

        static void RemoveWires(IOPort port)
        {
            port.wires.Where(w => w != activeWire).ToList().ForEach(w => w.Remove());
            port.wires.Clear();
        }
    }
    public void RefreshWire()
    {
        wires.ForEach(w =>
        {
            bool? signal = w.Output == null ? (w.Input?.GetSignal()) : (w.Output?.GetSignal());
            w.Draw(WireConnection, signal);
        });
    }

    private void ShowSprite(bool visible)
    {
        if (portType == EPortType.Input && wires.Count == 0)
            Sprite.BeginAnimation(OpacityProperty,
                new DoubleAnimation(fromValue: visible ? 0 : 1,
                                    toValue:   visible ? 1 : 0,
                                    duration:  TimeSpan.FromSeconds(0.5)));
    }

    private void Wire_MouseMove(object sender, MouseEventArgs e)
    {
        if (!isPressed) 
            return;
        // Redraw if there user is dragging the logic gate, and theres an active wire
        activeWire?.Draw(PointToScreen(e.GetPosition(this)), GetSignal());
    }
    private void Wire_MouseUp(object sender, MouseEventArgs e)
    {
        isPressed = false;
        this.MainGrid().MouseMove -= Wire_MouseMove;
        this.MainGrid().MouseUp -= Wire_MouseUp;

        bool wireIsHalfConneceted = activeWire?.ConnectedPorts.ContainsKey(portType == EPortType.Output ? EPortType.Input : EPortType.Output) == false;
        if (wireIsHalfConneceted)
        {
            //this.MainWindow().OpenCreationMenu(PointToScreen(e.GetPosition(this)), portType);
            if (activeWire != null)
            {
                activeWire.Remove();
                wires.Remove(activeWire);
                activeWire = null;
            }
            ShowSprite(true);
        }
    }
    private void Grid_Loaded(object sender, RoutedEventArgs e)
    {
        if (portType == EPortType.Output)
            Sprite.Margin = new Thickness(OverlapDetector.Width / 6, OverlapDetector.Height / 6, 0, 0);
    }
}