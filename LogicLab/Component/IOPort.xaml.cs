﻿using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.Immutable;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using LogicLab.Component;

namespace LogicLab;

public enum EPortType 
{ 
    Input, 
    Output 
}

// Austin (entire class)     
// Purpose: Can accept a wire. One input IO port connects to one Output IO port. 
// Logic Components can have multiple inputs, but only one outport, at max. 
public  partial class IOPort : UserControl
{
    public Point WireConnection
    {
        get
        {
            try
            {
                return PointToScreen(new(
                    Sprite.Margin.Left + (portType == EPortType.Input ? +(Parent is StackPanel _ ? ActualWidth / 2 : ActualWidth / 4) : 0),
                    Sprite.Margin.Right + ActualHeight / 2));
            }
            catch { return new Point(0, 0); }
        }
    }

    // If this is an input port, this will have at max one wire, and that wire will be connected to one output port This returns that one output port. 
    // If this is an output port, it can have an indefinite amount of wires each connected to their own input port. This returns all connected input port. 
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
            return [.. connectedPorts];
        }
    }
    public bool Connectionless => wires.Count == 0;
    
    private readonly EPortType portType;
    private readonly LogicComponent owningComponent;
    private readonly SolidColorBrush idleColor, hoverColor;
    private readonly List<Wire> wires = [];

    private static Wire? activeWire;
    private bool? _signal = null;
    private bool isPressed;

    public IOPort(EPortType portType, LogicComponent owner)
    {
        InitializeComponent();

        owningComponent    = owner;
        idleColor          = new SolidColorBrush(Color.FromRgb(9, 180, 255));
        hoverColor         = new SolidColorBrush(Color.FromRgb(59, 230, 255));
        this.portType      = portType;

        var bitmapImage    = new BitmapImage(new Uri($"Images/{this.portType}.png", UriKind.Relative));
        var imageBrush     = new ImageBrush(bitmapImage);
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

    public void RemoveWire(Wire wire, bool updateSprite = true)
    {
        wires.Remove(wire);
        if (updateSprite)       // Show the input port sprite that gets hidden when a wire is connected
            ShowSprite(true);
    }
    public void RemoveAllWires(bool updateSprite = true)
    {
        wires.ToImmutableList().ForEach(w => w.Remove(updateSprite));
        wires.Clear();

        if (updateSprite) 
            ShowSprite(true);
    }
    public void SetWireVisibility(Visibility visibility) // GA
    {
        foreach (Wire wire in wires)
            wire.Visibility = visibility;
        Visibility = visibility;
    }
    // Force all wires to redraw themselves
    public void RefreshWires()
    {
        wires.ForEach(w =>
        {
            bool? signal = w.Output == null ? (w.Input?.GetSignal()) : (w.Output?.GetSignal());
            w.Draw(WireConnection, signal);
        });
    }
    // Changes the signals of ports, and propagate that signal outward
    public void SetSignal(bool? value)
    {
        // Don't set signal if it results in no change. This prevents some trival infinite loops
        if (value == _signal)   
            return;

        _signal = value;
        owningComponent.ShowSignal(value);
        ProcessSignalAsync(value);
    }
    public bool? GetSignal() => _signal;
    public async void RefreshWiresAsync()
    {
        await Task.Delay(1);      // This ensures the state is correct
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
        // User was not dragging a wire
        if (activeWire == null)
            return;
        // User tried to drag an Output onto an output
        if (portType == EPortType.Output && activeWire.Output != null)
            return;
        // User tried to drag an input onto an input
        if (portType == EPortType.Input && activeWire.Input != null)
            return;

        // Hide sprite if port is input
        ShowSprite(false);

        if (activeWire.Input == null)   // User is connecting a wire from an output port to this input port
            RemoveAllWires(false);
        else   // User is connecting a wire from an input port to this output port
        {
            RemoveWires(activeWire.Input);
            activeWire.Input.wires.Add(activeWire);
        }

        // If the user releases their mouse on this port, and there's an active wire
        // connect that wire to this port, making a fully connected wire
        activeWire.SetPort(portType, this);
        wires.Add(activeWire);

        if (activeWire.Output != null)
            SetSignal(activeWire.Output.GetSignal());
        activeWire.Input?.SetSignal(GetSignal());
        activeWire.Draw(WireConnection, GetSignal());

        static void RemoveWires(IOPort port)
        {
            port.wires.Where(w => w != activeWire).ToList().ForEach(w => w.Remove());
            port.wires.Clear();
        }
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
    
    private async void ProcessSignalAsync(bool? signal)
    {
        if (portType == EPortType.Input)
            owningComponent.OnInputChange(this);        
        else  // If Output
        {
            await Task.Delay(100);
            // In cases where the signal turns itself on and off, like a handmade XOR gate, it causes an infinte loop of never ending updates
            // This delay allows some time to pass, thus not blocking the thread. The result is a flicker effect, which was the goal. 

            foreach (var port in ConnectedPorts)
                if (port.portType == EPortType.Input)
                    port.SetSignal(signal);
            foreach (var wire in wires)
                if (wire.Output == this)
                    wire.ShowSignal(signal);
        }
    }
    private void ShowSprite(bool visible)
    {
        if (portType == EPortType.Input && wires.Count == 0)
            Sprite.BeginAnimation(OpacityProperty,
                new DoubleAnimation(fromValue: visible ? 0 : 1,
                                    toValue:   visible ? 1 : 0,
                                    duration:  TimeSpan.FromSeconds(0.5)));
    }
    private void ShowSignal(bool? signal) => owningComponent.ShowSignal(signal);
    private void UpdateBackground(bool? signal)
    {
        if (owningComponent is LogicGate logicGate)
            logicGate.BackgroundSprite.Fill =
                signal == true ? new SolidColorBrush(Color.FromRgb(150, 150, 30))
                               : new SolidColorBrush(Color.FromRgb(30, 30, 30));
    }
}