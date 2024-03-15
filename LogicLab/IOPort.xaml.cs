using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Input;

namespace LogicLab;

public enum EPortType { Input, Output }

public partial class IOPort : UserControl
{
    // TODO:: Remove dependency prop
    public static readonly DependencyProperty PortTypeProperty =
        DependencyProperty.Register("PortType", typeof(EPortType), typeof(IOPort), new PropertyMetadata(EPortType.Input));

    public EPortType PortType
    {
        get => (EPortType)GetValue(PortTypeProperty);
        set => SetValue(PortTypeProperty, value);
    }

    public Grid MainGrid => (Window.GetWindow(this) as MainWindow)?.MainGrid ?? throw new NullReferenceException("Null ref");

    private readonly SolidColorBrush idleColor, hoverColor;
    private bool isPressed;

    public IOPort(EPortType portType)
    {
        InitializeComponent();

        idleColor  = new SolidColorBrush(Color.FromRgb(9, 180, 255));
        hoverColor = new SolidColorBrush(Color.FromRgb(59, 230, 255));
        PortType   = portType;

        BitmapImage bitmapImage = new(new Uri($"Images/{PortType}.png", UriKind.Relative));
        ImageBrush imageBrush = new(bitmapImage);

        Sprite.Fill = idleColor;
        Sprite.OpacityMask = imageBrush;

        if (portType == EPortType.Input)
            return;

        // You may need to adjust the hover size as per your requirements
        Sprite.Height *= 2.5;
        Sprite.Width *= 1.75;
    }

    private void Sprite_MouseEnter(object sender, MouseEventArgs e)
    {
        Sprite.Fill = hoverColor;
    }

    private void Sprite_MouseLeave(object sender, MouseEventArgs e)
    {
        Sprite.Fill = idleColor;
    }

    private void Wire_MouseMove(object sender, MouseEventArgs e)
    {
        if (!isPressed) 
            return;
        DebugLabel.Content = e.GetPosition(this);
    }
    private void Wire_MouseUp(object sender, MouseEventArgs e)
    {
        isPressed = false;
        MainGrid.MouseMove -= Wire_MouseMove;
        MainGrid.MouseUp -= Wire_MouseUp;
    }

    private void Sprite_MouseDown(object sender, MouseEventArgs e)
    {
        isPressed = true;
        MainGrid.MouseMove += Wire_MouseMove;
        MainGrid.MouseUp += Wire_MouseUp;
    }
}