using System.Collections.Immutable;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using LogicLab.Component;

namespace LogicLab;

// Austin
public abstract partial class LogicComponent : LabComponent
{
    public Rectangle BackgroundSprite { get; private set; }

    protected abstract Grid ControlGrid { get; }
    protected virtual ImmutableList<IOPort> InputPorts => inputPorts.ToImmutableList();
    protected bool IsSelected          => ComponentSelector.IsSelected(this);
    protected IOPort InputPort         => InputPorts[0];
    protected IOPort? OutputPort       => outputPort;
    protected List<bool?> InputSignals => InputPorts.Select(ip => ip.GetSignal()).ToList();
    protected ComponentDragger Dragger;

    private IOPort? outputPort;
    private readonly List<IOPort> inputPorts = [];

    private static int count = 0;
    public int ID { get; private set; }

    public LogicComponent() : base()
    {
        ID               = count++;
        Dragger          = new ComponentDragger(this);   // Enables dragging
        BackgroundSprite = new()
        {
            Width   = 50,
            Height  = 50,
            Fill    = new SolidColorBrush(Color.FromRgb(25, 25, 25)),
            RadiusX = 5,
            RadiusY = 5
        };

        Deselect();
    }

    public void Select(bool shiftSelect)
    {
        if (IsSelected)
            return;
        //"Select".Show();
        BeginShadowAnimation(shadow, highlight);

        if (shiftSelect)
             ComponentSelector.ShiftSelect(this);
        else ComponentSelector.SingleSelect(this);
    }
    public override void ShowSignal(bool? signal)
    {
        Color targetColor = signal == true
            ? Color.FromRgb(150, 150, 30)
            : signal == null
            ? Color.FromRgb(200, 50, 50)
            : Color.FromRgb(25, 25, 25);
        BackgroundSprite.Fill.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(targetColor, TimeSpan.FromSeconds(0.25)));
        OutputPort?.RefreshWire();
    }
    public virtual void Deselect()
    {
        BeginShadowAnimation(highlight, shadow);
        ComponentSelector.Deselect(this);
    }
    public virtual void OnInputChange(IOPort changedPort, List<SignalPath> propagationHistory) { }
    public virtual void OnDelete() { }
    public void OnDrag(MouseEventArgs e)
    {
        InputPorts.ForEach(io => io.OnDrag());
        OutputPort?.OnDrag();
    }

    protected IOPort AddInputPort(IAddChild addChild)
    {
        IOPort port = new(EPortType.Input, this);
        addChild.AddChild(port);
        inputPorts.Add(port);
        
        return port;
    }
    protected IOPort AddOutputPort(IAddChild addChild)
    {
        IOPort port = new(EPortType.Output, this);
        addChild.AddChild(port);
        outputPort = port;

        return port;
    }
    
    protected virtual void Component_MouseDown (object sender, MouseButtonEventArgs e)
    {
        if (!IsSelected && ShiftKey)
            Select(shiftSelect: true);
        else if (!IsSelected)
        {
            ComponentSelector.DeselectAll(this);
            Select(shiftSelect: false);
        }

        Dragger?.DragStart(e);
    }
    protected virtual void Component_MouseUp   (object sender, MouseButtonEventArgs e) => Dragger?.DragEnd();
    protected virtual void Component_MouseMove (object sender, MouseEventArgs e)       => Dragger?.DragMove(e);
    protected virtual void Grid_Loaded         (object sender, RoutedEventArgs e)
    {
        // Subscribe to mouse move, that way the mouse move will fire even if the cursor goes out of bounds, when dragging
        if (Window.GetWindow(this) is MainWindow mw)
            if (mw.MainGrid != null)
                mw.MainGrid.MouseMove += Component_MouseMove;
        if (BackgroundSprite.Parent == null)
            ControlGrid.Children.Insert(0, BackgroundSprite);
    }

    private void BeginShadowAnimation(DropShadowEffect fromShadow, DropShadowEffect targetShadow)
    {
        // Animates from shadow to highlight or back
        TimeSpan animTime = TimeSpan.FromSeconds(0.25);

        animatingShadow = fromShadow.Clone();
        animatingShadow.BeginAnimation(DropShadowEffect.ShadowDepthProperty, new DoubleAnimation(targetShadow.ShadowDepth, animTime));
        animatingShadow.BeginAnimation(DropShadowEffect.ColorProperty,       new ColorAnimation (targetShadow.Color,       animTime));
        animatingShadow.BeginAnimation(DropShadowEffect.OpacityProperty,     new DoubleAnimation(targetShadow.Opacity,     animTime));
        animatingShadow.BeginAnimation(DropShadowEffect.BlurRadiusProperty,  new DoubleAnimation(targetShadow.BlurRadius,  animTime));
        Effect = animatingShadow;
    }
}//113, 83