using System;
using System.Diagnostics;
using System.Security.Policy;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace LogicLab;

public enum ESignal { Off = 0, On = 1 };

public partial class LogicGate : LogicComponent
{
    private ELogicGate gateType;

    private static List<ELogicGate> deleteMe;
    private static int count = 0;
    private int inputCount = 2;
    private double startHeight;
    private IOPort outputPort;

    private readonly DropShadowEffect outputGlow = new()
    {
        ShadowDepth = 0, Color = Colors.Blue, Opacity = 1, BlurRadius = 10
    }; 

    public LogicGate()
    {
        InitializeComponent();
        deleteMe = [
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

        //for (int i = 0; i < Grid.Children.Count; i++)
            //Grid.Children[i].Visibility = Visibility.Hidden;
        
        Random random = new();
        int randomNumber = random.Next();

        if (randomNumber % 2 == 0)
        {
            LinearGradientBrush gradientBrush = new()
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 1)
            };
            gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(0, 60, 120), 0));
            gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(0, 10, 20), 1));

            BackgroundSprite.Fill = gradientBrush;

        }

        //deleteMe = [ELogicGate.Buffer,
        //    ELogicGate.AND,
        //    ELogicGate.OR,
        //    ELogicGate.XOR,
        //    ELogicGate.NAND,
        //    ELogicGate.NOR,
        //    ELogicGate.XNOR];


    }

    public void Negate() => gateType = !gateType; 

    protected override void Gate_MouseDown (object sender, MouseButtonEventArgs e) => base.Gate_MouseDown(sender, e);
    protected override void Gate_MouseMove (object sender, MouseEventArgs e)       => base.Gate_MouseMove(sender, e);
    protected override void Gate_MouseUp   (object sender, MouseButtonEventArgs e) => base.Gate_MouseUp(sender, e);

    private void SetInputAmount(int amount)
    {
        if (gateType.IsSingleInput())
             inputCount = 1;
        else inputCount = amount;

        int extraInputs = Math.Max(0, inputCount - 2);
            
        BackgroundSprite.Height = startHeight + extraInputs * 20;

        for (int i = 0; i < inputCount; i++)
            PortPanel.Children.Add(new IOPort(EPortType.Input));
    }
    private void ShowImage()
    {
        BitmapImage bitmapImage = new(new Uri($"Images/{gateType} Gate.png", UriKind.Relative));
        ImageBrush imageBrush = new(bitmapImage);
        Sprite.Fill = imageBrush;
    }
    public override void OnDrag(MouseEventArgs e)
    {
        PortPanel.Children.OfType<IOPort>().ToList().ForEach(io => io.OnDrag(e));
        outputPort.OnDrag(e);
    }

    private void LogicComponent_Loaded(object sender, RoutedEventArgs e)
    {
        startHeight = ActualHeight;

        gateType = deleteMe[count];
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