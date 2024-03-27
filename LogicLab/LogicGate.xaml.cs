using System.Collections.Immutable;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace LogicLab;

public enum ESignal { Off = 0, On = 1 };    // This will be deleted, replaced with a WireSignalClass

public partial class LogicGate : LogicComponent
{
    public ELogicGate GateType { get; private set; }

    private bool? OutputSignal => GateType.ApplyGate(InputSignals);
    //private bool OutputSignal => gateType == ELogicGate.NOT ? true : gateType.ApplyGate(InputSignals);
    private static List<ELogicGate> thisWillBeDeletedLater; // This is just a placeholder, will be replaced by the logic gate creator menu
    private static int count = 0;                           // I have no clue what this does, but it will also be deleted later.
    private int inputCount = 2;                             // # of inputs, default is 2, but NOT and BUFFER have a min and max of 1
    private double startHeight;                             // Logic gates grow based on input size, this caches the startsize
    private IOPort outputPort;
    private bool canResize;
    private bool isResizing;
    private Point startResize;
    private bool mouseDown;
    protected override Grid ControlGrid => Grid;

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
    }

    public void Negate()
    {
        GateType = !GateType;
        NegateDot.Visibility = GateType.IsNegative() ? Visibility.Visible : Visibility.Hidden;
    }
    public override void OnInputChange(IOPort changedPort) => OutputPort.Signal = OutputSignal;


    // Forward events to Logic Component to get highlight and drop shadow features.
    protected override void Component_MouseDown (object sender, MouseButtonEventArgs e) => base.Component_MouseDown(sender, e);
    protected override void Component_MouseMove (object sender, MouseEventArgs e)       => base.Component_MouseMove(sender, e);
    protected override void Component_MouseUp   (object sender, MouseButtonEventArgs e) => base.Component_MouseUp(sender, e);

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
    private void ShowImage()
    {
        BitmapImage bitmapImage = new(new Uri($"Images/{GateType} Gate.png", UriKind.Relative));
        Sprite.Fill = new ImageBrush(bitmapImage);
    }
    private void Gate_MouseMove(object sender, System.Windows.Input.MouseButtonEventArgs e)
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
                    BackgroundSprite.Height -= InputPort.ActualHeight;

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
        else
        {
            isResizing = true;
            startResize = e.GetPosition(this);
        }
    }
    private void Background_MouseUp(object sender, MouseButtonEventArgs e)
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
    
    private void LogicComponent_Loaded(object sender, RoutedEventArgs e)
    {
        startHeight = 50;

        // This is super temp
        GateType = thisWillBeDeletedLater[count];
        BackgroundSprite.MouseDown += Background_MouseDown;
        BackgroundSprite.MouseMove += Background_MouseMove;
        BackgroundSprite.MouseUp   += Background_MouseUp;
        
        // Create and organize ports
        SetInputAmount(2 + (int)Math.Floor(count / 4.0));
        count++;
        AddOutputPort(OutputPanel);

        ShowImage();
        Random random = new();
        int randomNumber = random.Next();

        // This randomly assigns some logic gates to be automically on. This is incredibly placeholder. 
        if (randomNumber % 2 == 0 && GateType == ELogicGate.Buffer)
        {
            Negate();
            OutputPort.Signal = true;
            OnInputChange(InputPanel.Children[0] as IOPort);
        }
        if (GateType == ELogicGate.Buffer)
            OutputPort.Signal = false;

        this.MainGrid().MouseMove += Background_MouseMove;
        this.MainGrid().MouseUp += Background_MouseUp;
    }
}