﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Input;

namespace LogicLab;

public enum EPortType { Input, Output }

public partial class IOPort : UserControl
{
    public Point EndPoint => PointToScreen(new(
                Sprite.Margin.Left + (portType == EPortType.Input ? ActualWidth : 0),
                Sprite.Margin.Right + ActualHeight / 2));
    private EPortType portType;

    private readonly SolidColorBrush idleColor, hoverColor;
    private bool isPressed;

    private readonly List<Wire> wires = [];
    private static Wire? activeWire;


    public IOPort(EPortType portType)
    {
        InitializeComponent();

        idleColor     = new SolidColorBrush(Color.FromRgb(9, 180, 255));
        hoverColor    = new SolidColorBrush(Color.FromRgb(59, 230, 255));
        this.portType = portType;

        BitmapImage bitmapImage = new(new Uri($"Images/{this.portType}.png", UriKind.Relative));
        ImageBrush imageBrush = new(bitmapImage);

        Sprite.Fill = idleColor;
        Sprite.OpacityMask = imageBrush;

        if (portType == EPortType.Input)
            return;

        // You may need to adjust the hover size as per your requirements
        Sprite.Height *= 2.5;
        Sprite.Width *= 1.75;
    }
    private void Sprite_MouseEnter(object sender, MouseEventArgs e) => Sprite.Fill = hoverColor;
    private void Sprite_MouseLeave(object sender, MouseEventArgs e) => Sprite.Fill = idleColor;

    private void Wire_MouseMove(object sender, MouseEventArgs e)
    {
        if (!isPressed) 
            return;
        activeWire?.Draw(portType, PointToScreen(e.GetPosition(this)));
    }
    private void Wire_MouseUp(object sender, MouseEventArgs e)
    {
        isPressed = false;
        this.MainGrid().MouseMove -= Wire_MouseMove;
        this.MainGrid().MouseUp -= Wire_MouseUp;

        if (activeWire?.ConnectedPorts.ContainsKey(portType == EPortType.Output ? EPortType.Input : EPortType.Output) == false)
        {
            activeWire.Remove();
            wires.Remove(activeWire);
            activeWire = null;
        }
        //MessageBox.Show(activeWire.ConnectedPorts.ContainsKey(EPortType.Input).ToString());
    }

    private void Sprite_MouseDown(object sender, MouseEventArgs e)
    {
        isPressed = true;
        this.MainGrid().MouseMove += Wire_MouseMove;
        this.MainGrid().MouseUp += Wire_MouseUp;

        Wire wire = new(this.MainWindow(), EndPoint);
        wire.SetPort(portType, this);
        activeWire = wire;
        activeWire.Draw(portType, PointToScreen(e.GetPosition(this)));
        wires.Add(wire);
    }

    private void Sprite_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (activeWire == null)
            return;

        activeWire.SetEndPoint(portType, EndPoint);
        activeWire.SetPort(portType, this);
        wires.Add(activeWire);

        if (portType == EPortType.Input)
            Sprite.Visibility = Visibility.Hidden;
    }

    public void OnDrag(MouseEventArgs e)
    {
        this.DebugLabel().Content += wires.Count.ToString();
        //this.DebugLabel().Content = PointToScreen(e.GetPosition(this));
        wires.ForEach(w => w.SetEndPoint(portType, EndPoint));
    }

    private void Grid_Loaded(object sender, RoutedEventArgs e)
    {
        
    }
}