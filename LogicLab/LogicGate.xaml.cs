using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LogicLab;

public enum ESignal { Off = 0, On = 1 };    // This will be deleted, replaced with a WireSignalClass

public partial class LogicGate : LogicComponent
{
    private ELogicGate gateType;

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

        Random random = new();
        int randomNumber = random.Next();

        // This randomly assigns some logic gates to be automically on. This is incredibly placeholder. 
        if (randomNumber % 2 == 0)
        {
            // Gradiant version
            //LinearGradientBrush gradientBrush = new()
            //{
            //    StartPoint = new Point(0, 0),
            //    EndPoint = new Point(1, 1)
            //};
            //gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(0, 60, 120), 0));
            //gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(0, 10, 20), 1));

            // Solid version
            Negate();
            BackgroundSprite.Fill = new SolidColorBrush(Color.FromRgb(100, 30, 30));
        }

        // Ignore this
        //deleteMe = [ELogicGate.Buffer,
        //    ELogicGate.AND,
        //    ELogicGate.OR,
        //    ELogicGate.XOR,
        //    ELogicGate.NAND,
        //    ELogicGate.NOR,
        //    ELogicGate.XNOR];


    }

    public void Negate()
    {
        gateType = !gateType;
        NegateDot.Visibility = gateType.IsNegative() ? Visibility.Visible : Visibility.Hidden;
    }

    // Forward events to Logic Component to get highlight and drop shadow features.
    protected override void Gate_MouseDown (object sender, MouseButtonEventArgs e) => base.Gate_MouseDown(sender, e);
    protected override void Gate_MouseMove (object sender, MouseEventArgs e)       => base.Gate_MouseMove(sender, e);
    protected override void Gate_MouseUp   (object sender, MouseButtonEventArgs e) => base.Gate_MouseUp(sender, e);

    private void SetInputAmount(int amount)
    {
        if (gateType.IsSingleInput())   // Force NOT and BUFFER 1 input
             inputCount = 1;
        else inputCount = amount;

        // This is 0 if the gate has 2 inputs, reflecting that it should be the start size
        // Since we've cached the start size, this will also work when decrement input count
        int extraInputs = Math.Max(0, inputCount - 2);
            
        BackgroundSprite.Height = startHeight + extraInputs * 20;

        for (int i = 0; i < inputCount; i++)
            AddInputPort(PortPanel);
    }
    private void ShowImage()
    {
        // Loads the correct sprite. Will eventually need to show negative gates
        BitmapImage bitmapImage = new(new Uri($"Images/{gateType} Gate.png", UriKind.Relative));
        Sprite.Fill = new ImageBrush(bitmapImage);
    }
    public override void OnDrag(MouseEventArgs e)
    {
        // Forward drag function to input and output ports
        InputPorts.ForEach(io => io.OnDrag(e));
        outputPort.OnDrag(e);
    }

    private void LogicComponent_Loaded(object sender, RoutedEventArgs e)
    {
        startHeight = ActualHeight;

        // This is super temp
        gateType = thisWillBeDeletedLater[count];
        
        // Create and organize ports
        SetInputAmount(2 + (int)Math.Floor(count / 4.0));
        count++;
        outputPort = new IOPort(EPortType.Output);
        Grid.Children.Add(outputPort);
        outputPort.VerticalAlignment = VerticalAlignment.Center;

        ShowImage();
    }

    private void Gate_MouseMove(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {

    }
}