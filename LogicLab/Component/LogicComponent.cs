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
// Purpose: Parent class for Logic Gates, Inputs and Outputs
// Anthing that owns IO Ports is a logic component
public abstract partial class LogicComponent : LabComponent
{
    public Rectangle BackgroundSprite { get; private set; }
    protected abstract Grid ControlGrid { get; }
    protected virtual Rectangle? ForegroundSprite { get; } = null;

    protected virtual ImmutableList<IOPort> InputPorts => inputPorts.ToImmutableList();

    protected bool IsSelected          => ComponentSelector.IsSelected(this);
    protected IOPort? InputPort        => InputPorts.Count > 0 ? InputPorts[0] : null;
    protected IOPort? OutputPort       => outputPort;
    protected List<bool?> InputSignals => InputPorts.Select(ip => ip.GetSignal()).ToList();
    protected ComponentDragger Dragger;

    private IOPort? outputPort;
    private readonly List<IOPort> inputPorts = [];

    public LogicComponent() : base()
    {
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

    // Connor
    protected void AlignLeft       (object sender, RoutedEventArgs e) => ComponentSelector.AlignLeft();
    protected void AlignTop        (object sender, RoutedEventArgs e) => ComponentSelector.AlignTop();
    protected void AlignRight      (object sender, RoutedEventArgs e) => ComponentSelector.AlignRight();
    protected void AlignBottom     (object sender, RoutedEventArgs e) => ComponentSelector.AlignBottom();
    protected void AlignCenter     (object sender, RoutedEventArgs e) => ComponentSelector.AlignCenter();
    protected void DeleteComponent (object sender, RoutedEventArgs e)
    {
        if (IsSelected) // AB
            ComponentSelector.DeleteComponent();    // Connor
        // AB
        else
        {
            this.MainGrid().Children.Remove(this);
            OnDelete();
        }
        // end AB
    }

    // end Connor

    // Austin
    public void OnDelete()
    {
        OutputPort?.RemoveAllWires();
        InputPorts.ForEach(io => io.RemoveAllWires());
    }

    // GA
    public void UpdateAllWires() => OutputPort?.RefreshWires();
    public void RemoveAllWires() 
    {
        OutputPort?.RemoveAllWires();
        foreach (IOPort port in InputPorts)
            port.RemoveAllWires();
    }
    public void SetPortVisibility(Visibility visibility)
    {
        foreach (IOPort port in InputPorts)
        {
            port.Visibility = visibility;
            port.SetWireVisibility(visibility);

            if (visibility == Visibility.Visible)
                port.RefreshWires();
        }
        if (OutputPort != null)
            OutputPort.Visibility = visibility;
    }
    // end GA

    public void Select(bool shiftSelect)
    {
        if (IsSelected)
            return;
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
        OutputPort?.RefreshWires();
    }
    public virtual void Deselect()
    {
        BeginShadowAnimation(highlight, shadow);
        ComponentSelector.Deselect(this);
    }
    public virtual void OnInputChange(IOPort changedPort, List<SignalPath> propagationHistory) { }
    public void RefreshWires()
    {
        InputPorts.ForEach(io => io.RefreshWires());
        OutputPort?.RefreshWires();
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
    
    protected virtual void Component_MouseLeftButtonDown (object sender, MouseButtonEventArgs e)
    {
        if (e.RightButton == MouseButtonState.Pressed)
            return;
        if (!IsSelected && ShiftKey)
            Select(shiftSelect: true);
        else if (!IsSelected)
        {
            ComponentSelector.DeselectAll(this);
            Select(shiftSelect: false);
        }
        Dragger?.DragStart(e);
    }
    protected virtual void Component_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Released)
            Dragger?.DragEnd();
    }
    protected virtual void Component_MouseMove (object sender, MouseEventArgs e) => Dragger?.DragMove(e);
    protected virtual void Grid_Loaded         (object sender, RoutedEventArgs e)
    {
        // Subscribe to mouse move, that way the mouse move will fire even if the cursor goes out of bounds, when dragging
        if (Window.GetWindow(this) is MainWindow mw)
            if (mw.MainGrid != null)
                mw.MainGrid.MouseMove += Component_MouseMove;
        if (BackgroundSprite.Parent == null)
            ControlGrid.Children.Insert(0, BackgroundSprite);
        if (ForegroundSprite != null)
            ForegroundSprite.IsHitTestVisible = false;

        ContextMenu contextMenu = new();

        MenuItem deleteItem = new() { Header = "Delete" };
        MenuItem alignItem  = new() { Header = "Align" };
        MenuItem leftItem   = new() { Header = "Left" };
        MenuItem topItem    = new() { Header = "Top" };
        MenuItem rightItem  = new() { Header = "Right" };
        MenuItem bottomItem = new() { Header = "Bottom" };
        MenuItem centerItem = new() { Header = "Center" };

        deleteItem.Click += DeleteComponent; 
        leftItem  .Click += AlignLeft;
        topItem   .Click += AlignTop; 
        rightItem .Click += AlignRight;
        bottomItem.Click += AlignBottom;
        centerItem.Click += AlignCenter;

        alignItem.Items.Add(leftItem);
        alignItem.Items.Add(topItem);
        alignItem.Items.Add(rightItem);
        alignItem.Items.Add(bottomItem);
        alignItem.Items.Add(centerItem);

        contextMenu.Items.Add(deleteItem);
        contextMenu.Items.Add(alignItem);

        BackgroundSprite.ContextMenu = contextMenu;
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