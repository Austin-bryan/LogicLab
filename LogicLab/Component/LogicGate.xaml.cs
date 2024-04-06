using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection.Metadata.Ecma335;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using LogicLab.Component;

namespace LogicLab;

public enum ESignal { Off = 0, On = 1 };    // This will be deleted, replaced with a WireSignalClass

// Austin
public partial class LogicGate : LogicComponent
{
    public ELogicGate GateType { get; private set; }
    protected override Grid ControlGrid => Grid;

    // Austin
    private bool? OutputSignal => GateType.ApplyGate(InputSignals);
    private static List<ELogicGate> thisWillBeDeletedLater; // This is just a placeholder, will be replaced by the logic gate creator menu
    private static int count = 0;                           // I have no clue what this does, but it will also be deleted later.
    
    private double startHeight;                             // Logic gates grow based on input size, this caches the startsize
    private int inputCount = 2;                             // # of inputs, default is 2, but NOT and BUFFER have a min and max of 1
    private bool canResize, isResizing, mouseDown, isCreatedViaDesigner;
    private Point startResize;
    private IOPort outputPort;

    public LogicGate()
    {
        InitializeComponent();
        // Dont worry about this too much, this is temp. Just using this to populate the world with different gate types.
        thisWillBeDeletedLater = [
            ELogicGate.Buffer, ELogicGate.AND, ELogicGate.OR, ELogicGate.XOR,
            ELogicGate.Buffer, ELogicGate.AND, ELogicGate.OR, ELogicGate.XOR,
            ELogicGate.Buffer, ELogicGate.AND, ELogicGate.OR, ELogicGate.XOR,
            ELogicGate.Buffer, ELogicGate.AND, ELogicGate.OR, ELogicGate.XOR,
            ELogicGate.Buffer, ELogicGate.AND, ELogicGate.OR, ELogicGate.XOR];

        isCreatedViaDesigner = true;

    }
    public LogicGate(ELogicGate logicGate)
    {
        InitializeComponent();
        GateType = logicGate;
    }

    public void Negate()
    {
        GateType = !GateType;
        NegateDot.Visibility = GateType.IsNegative() ? Visibility.Visible : Visibility.Hidden;
    }
    public override void OnInputChange(IOPort changedPort, List<IOPort> propagationHistory) => OutputPort.SetSignal(OutputSignal, propagationHistory);
    public override void ShowSignal(bool? signal) => base.ShowSignal(OutputSignal);

    // Forward events to Logic Component to get highlight and drop shadow features.
    protected override void Component_MouseDown (object sender, MouseButtonEventArgs e) => base.Component_MouseDown(sender, e);
    protected override void Component_MouseMove (object sender, MouseEventArgs e)       => base.Component_MouseMove(sender, e);
    protected override void Component_MouseUp   (object sender, MouseButtonEventArgs e) => base.Component_MouseUp(sender, e);

    // Connor
    private void AlignLeft  (object sender, RoutedEventArgs e) => ComponentSelector.AlignLeft();
    private void AlignTop   (object sender, RoutedEventArgs e) => ComponentSelector.AlignTop();
    private void AlignRight (object sender, RoutedEventArgs e) => ComponentSelector.AlignRight();
    private void AlignBottom(object sender, RoutedEventArgs e) => ComponentSelector.AlignBottom();
    private void AlignCenter(object sender, RoutedEventArgs e) => ComponentSelector.AlignCenter();

    // Austin
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
    private void Gate_MouseMove(object sender, MouseButtonEventArgs e)
    {

    }
    
    private void Background_MouseMove(object sender, MouseEventArgs e)
    {
        if (isResizing)
        {
            Vector delta = e.GetPosition(this) - startResize;

            double deltaY = Math.Abs(delta.Y);
            while (deltaY > InputPort.ActualHeight) 
            {
                if (delta.Y > 0)
                {
                    AddInputPort(InputPanel);
                    BackgroundSprite.Height += InputPort.ActualHeight;  
                }
                else if (TryRemoveEmptyInputPort())
                {
                    BackgroundSprite.Height -= InputPort.ActualHeight;
                    InputPanel.Children.ToList().OfType<IOPort>().ToList().ForEach(io => io.OnDrag());
                }

                deltaY -= Math.Sign(deltaY) * InputPort.ActualHeight; 
                startResize = e.GetPosition(this);
            }
        }
        else if (e.GetPosition(this).Y > ActualHeight - 10)
        {
            Cursor = Cursors.SizeNS;
            canResize = true;
        }
        else
        {
            Cursor = Cursors.SizeAll;
            canResize = false;
            Component_MouseMove(sender, e);
        }
    }
    private void Background_MouseDown(object sender, MouseButtonEventArgs e)
    {
        mouseDown = true;
      
        if (!canResize)
             Component_MouseDown(sender, e);
        else (isResizing, startResize) = (true, e.GetPosition(this));
    }
    private void Background_MouseUp  (object sender, MouseButtonEventArgs e)
    {
        if (!mouseDown)
            return;

        mouseDown = false;
        if (isResizing)
             isResizing = false;
        else Component_MouseUp(sender, e);
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

        // Create and organize ports
        // This is super temp
        if (isCreatedViaDesigner)
        {
            GateType = thisWillBeDeletedLater[count];
            SetInputAmount(2 + (int)Math.Floor(count / 4.0));
            count++;
        }
        else SetInputAmount(2);

        AddOutputPort(OutputPanel);
        ShowImage();

        this.MainGrid().MouseMove += Background_MouseMove;
        this.MainGrid().MouseUp += Background_MouseUp;
    }
}