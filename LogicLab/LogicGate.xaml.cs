using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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

    public LogicGate()
    {
        InitializeComponent();
        // Dont worry about this too much, this is temp. Just using this to populate the world with different gate types.
        thisWillBeDeletedLater = [
            ELogicGate.Buffer,
            ELogicGate.AND,
            ELogicGate.OR,
            ELogicGate.XOR,
            ELogicGate.Buffer,
            ELogicGate.AND,
            ELogicGate.OR,
            ELogicGate.XOR,
            ELogicGate.Buffer,
            ELogicGate.AND,
            ELogicGate.OR,
            ELogicGate.XOR,
            ELogicGate.Buffer,
            ELogicGate.AND,
            ELogicGate.OR,
            ELogicGate.XOR,
            ELogicGate.Buffer,
            ELogicGate.AND,
            ELogicGate.OR,
            ELogicGate.XOR];
    }

    public void Negate()
    {
        GateType = !GateType;
        NegateDot.Visibility = GateType.IsNegative() ? Visibility.Visible : Visibility.Hidden;
    }

    // Forward events to Logic Component to get highlight and drop shadow features.
    protected override void Gate_MouseDown (object sender, MouseButtonEventArgs e) => base.Gate_MouseDown(sender, e);
    protected override void Gate_MouseMove (object sender, MouseEventArgs e)       => base.Gate_MouseMove(sender, e);
    protected override void Gate_MouseUp   (object sender, MouseButtonEventArgs e) => base.Gate_MouseUp(sender, e);

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
    public override void OnInputChange(IOPort changedPort)
    {
        OutputPort.Signal = OutputSignal;
    }

    private void AlignRight(object sender, RoutedEventArgs e) => ComponentSelector.AlignRight();
    private void AlignTop(object sender, RoutedEventArgs e) => ComponentSelector.AlignTop();
    private void AlignLeft(object sender, RoutedEventArgs e) => ComponentSelector.AlignLeft();
    private void AlignCenter(object sender, RoutedEventArgs e) => ComponentSelector.AlignCenter();
    private void AlignBottom(object sender, RoutedEventArgs e) => ComponentSelector.AlignBottom();
    private void ShowImage()
    {
        // Loads the correct sprite. Will eventually need to show negative gates
        BitmapImage bitmapImage = new(new Uri($"Images/{GateType} Gate.png", UriKind.Relative));
        Sprite.Fill = new ImageBrush(bitmapImage);
    }
    public override void OnDrag(MouseEventArgs e)
    {
        // Forward drag function to input and output ports
        InputPorts.ForEach(io => io.OnDrag());
        OutputPort.OnDrag();
    }

    private void LogicComponent_Loaded(object sender, RoutedEventArgs e)
    {
        startHeight = ActualHeight;

        // This is super temp
        GateType = thisWillBeDeletedLater[count];
        
        // Create and organize ports
        SetInputAmount(2 + (int)Math.Floor(count / 4.0));
        count++;
        AddOutputPort(OutputPanel);
        //OutputPort.SetTop(ActualHeight / 2 - OutputPort.ActualHeight / 2);
        //OutputPort.SetLeft(BackgroundSprite.Width);

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
    }

    private void Gate_MouseMove(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {

    }
}