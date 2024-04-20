using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.Immutable;
using LogicLab.Component;
using System.Windows.Shapes;

namespace LogicLab;

public enum ESignal { Off = 0, On = 1 };    // This will be deleted, replaced with a WireSignalClass

// Austin
// Purpose: This is the main logic component, the one that preforms boolean operations. It can be resized, have an indefinite amount of inputs,
// but always has one output.
public partial class LogicGate : LogicComponent
{
    public ELogicGate GateType { get; private set; }
    protected override Grid ControlGrid => Grid;
    protected override ImmutableList<IOPort> InputPorts => InputPanel.Children.ToList().OfType<IOPort>().ToImmutableList();
    protected override Rectangle ForegroundSprite => Sprite;

    // Austin
    private bool? OutputSignal => GateType.ApplyGate(InputSignals);
    private double startHeight;                             // Logic gates grow based on input size, this caches the startsize
    private int    inputCount = 2;                          // # of inputs, default is 2, but NOT and BUFFER have a min and max of 1
    private bool   canResize, isResizing, mouseDown;
    private Point  startResize;

    public LogicGate(ELogicGate logicGate)
    {
        InitializeComponent();
        GateType = logicGate;
    }

    // Called whenever the state of one if its input ports has changed, then updates its output signal
    public async override void OnInputChange(IOPort changedPort)
    {
        await Task.Delay(1);    // This avoids a particular glitch with the XOR gate
        OutputPort?.SetSignal(OutputSignal);
    }

    public override void ShowSignal(bool? signal) => base.ShowSignal(OutputSignal);

    // Forward events to Logic Component to get highlight and drop shadow features.
    protected override void Component_MouseLeftButtonDown (object sender, MouseButtonEventArgs e) => base.Component_MouseLeftButtonDown(sender, e);
    protected override void Component_MouseLeftButtonUp   (object sender, MouseButtonEventArgs e) => base.Component_MouseLeftButtonUp(sender, e);
    protected override void Component_MouseMove (object sender, MouseEventArgs e)                 => base.Component_MouseMove(sender, e);

    private void ShowImage()
    {
        Sprite.Fill = GateType.GetImage();
        NegateDot.Visibility = GateType.IsNegative() ? Visibility.Visible : Visibility.Hidden;
    }
    private void SetInputAmount(int amount)
    {
        if (GateType.IsSingleInput())   // Force NOT and BUFFER 1 input
             inputCount = 1;
        else inputCount = amount;

        // This is 0 if the gate has 2 inputs, reflecting that it should be the start size
        // Since we've cached the start size, this will also work when decrement input count
        int extraInputs = Math.Max(0, inputCount - 2);
            
        BackgroundSprite.Height = startHeight + extraInputs * 20;

        for (int i = 0; i < inputCount; i++)
            AddInputPort(InputPanel);
    }
    
    private async void Background_MouseMove(object sender, MouseEventArgs e)
    {
        if (isResizing && InputPort != null)
        {
            Vector delta = e.GetPosition(this) - startResize;

            double deltaY = Math.Abs(delta.Y);
            
            // If the mouse moved 100 px, and the ActualHeight is 30, this will add or remove 3 input ports, as 30 goes into 100, 3 times.
            // This allows smooth dynamic adding or removing of input ports
            while (deltaY > InputPort.ActualHeight) 
            {
                if (delta.Y > 0)
                {
                    AddInputPort(InputPanel);
                    if (InputPanel.Children.Count == 3)
                         BackgroundSprite.Height = 64;
                    else BackgroundSprite.Height += InputPort.ActualHeight;
                }
                else if (TryRemoveEmptyInputPort())     // Returns true only if an empty input port was able to be removed
                {
                    if (InputPanel.Children.Count == 2)
                         BackgroundSprite.Height = 50;
                    else BackgroundSprite.Height -= InputPort.ActualHeight;

                    InputPorts.ForEach(io => io.RefreshWiresAsync());
                }

                deltaY -= Math.Sign(deltaY) * InputPort.ActualHeight; 
                startResize = e.GetPosition(this);

                ShowSignal(OutputSignal);
                OutputPort?.SetSignal(OutputSignal);

                await Task.Delay(1);
                OutputPort?.RefreshWires();
            }
        }
        else if (e.GetPosition(this).Y > ActualHeight - 10 && GateType.PositiveGate() != ELogicGate.Buffer)
            (Cursor, canResize) = (Cursors.SizeNS, true);
        else
        {
            (Cursor, canResize) = (Cursors.SizeAll, false);
            Component_MouseMove(sender, e);
        }
    }
    private void Background_MouseDown(object sender, MouseButtonEventArgs e)
    {
        mouseDown = true;
      
        if (!canResize)
             Component_MouseLeftButtonDown(sender, e);
        else (isResizing, startResize) = (true, e.GetPosition(this));
    }
    private void Background_MouseUp  (object sender, MouseButtonEventArgs e)
    {
        if (!mouseDown)
            return;

        mouseDown = false;
        if (isResizing)
             isResizing = false;
        else Component_MouseLeftButtonUp(sender, e);
    }
    private bool TryRemoveEmptyInputPort()
    {
        if (InputPanel.Children.Count <= 2)
            return false;
        var ports = InputPanel.Children.ToList()
                                       .OfType<IOPort>()
                                       .ToImmutableList()
                                       .Reverse();
        foreach (var inputPort in ports)
        {
            if (inputPort.Connectionless)
            {
                InputPanel.Children.Remove(inputPort);
                return true;
            }
        }
        return false;
    }
    
    // Austin
    private void LogicComponent_Loaded(object sender, RoutedEventArgs e)
    {
        startHeight = 50;
        BackgroundSprite.MouseDown  += Background_MouseDown;
        BackgroundSprite.MouseMove  += Background_MouseMove;
        BackgroundSprite.MouseUp    += Background_MouseUp;
        BackgroundSprite.ContextMenu = Sprite.ContextMenu;

        SetInputAmount(2);

        AddOutputPort(OutputPanel);
        ShowImage();

        this.MainGrid().MouseMove += Background_MouseMove;
        this.MainGrid().MouseUp += Background_MouseUp;
    }
}