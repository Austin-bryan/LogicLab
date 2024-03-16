﻿using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LogicLab;

public partial class LogicGate : LogicComponent
{
    private ELogicGate gateType;

    private static List<ELogicGate> deleteMe;
    private static int count = 0;
    private int inputCount = 2;
    private double startHeight;
    private IOPort outputPort;

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

        //deleteMe = [ELogicGate.Buffer,
        //    ELogicGate.AND,
        //    ELogicGate.OR,
        //    ELogicGate.XOR,
        //    ELogicGate.NAND,
        //    ELogicGate.NOR,
        //    ELogicGate.XNOR];

   
    }

    public void Negate() => gateType = !gateType; 

    protected override void Gate_MouseDown (object sender, System.Windows.Input.MouseButtonEventArgs e) => base.Gate_MouseDown(sender, e);
    protected override void Gate_MouseMove (object sender, System.Windows.Input.MouseEventArgs e)       => base.Gate_MouseMove(sender, e);
    protected override void Gate_MouseUp   (object sender, System.Windows.Input.MouseButtonEventArgs e) => base.Gate_MouseUp(sender, e);

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
        GateImage.Fill = imageBrush;
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
        Grid.Children.Insert(0, outputPort);
        outputPort.VerticalAlignment = VerticalAlignment.Center;

        ShowImage();
    }

    private void Gate_MouseMove(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {

    }
}