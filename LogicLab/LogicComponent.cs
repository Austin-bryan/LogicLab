﻿using System.Collections.Immutable;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace LogicLab;

public abstract partial class LogicComponent : UserControl
{
    private static bool ShiftKey => Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
    private bool IsSelected => ComponentSelector.IsSelected(this);

    protected ComponentDragger? Dragger;
    protected IOPort InputPort                 => InputPorts[0];
    protected IOPort OutputPort                => outputPort;
    protected List<bool?> InputSignals         => InputPorts.Select(ip => ip.Signal).ToList();
    protected ImmutableList<IOPort> InputPorts => inputPorts.ToImmutableList();

    private IOPort outputPort;
    private readonly List<IOPort> inputPorts = [];
    private readonly DropShadowEffect shadow = new() { ShadowDepth = 4, Color = Colors.Black, Opacity = 0.4, BlurRadius = 5 };
    private readonly DropShadowEffect highlight = new() { ShadowDepth = 0, Color = Color.FromRgb(200, 200, 255), Opacity = 1, BlurRadius = 10 };
    private DropShadowEffect animatingShadow;

    public LogicComponent()
    {
        animatingShadow = shadow;
        Dragger = new ComponentDragger(this);   // Enables dragging
        Deselect();
    }

    public abstract void ShowSignal(bool? signal);
    public void Select(bool shiftSelect)
    {
        if (IsSelected)
            return;
        BeginShadowAnimation(shadow, highlight);

        if (shiftSelect)
             ComponentSelector.ShiftSelect(this);
        else ComponentSelector.SingleSelect(this);
    }
    public virtual void Deselect()
    {
        BeginShadowAnimation(highlight, shadow);
        ComponentSelector.Deselect(this);
    }
    // Uses these methods to add input and output ports to logic components
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
    public virtual void OnInputChange(IOPort changedPort)
    {

    }
    public virtual void OnDrag(MouseEventArgs e) { }
    private void BeginShadowAnimation(DropShadowEffect fromShadow, DropShadowEffect targetShadow)
    {
        // Animates from shadow to highlight or back
        TimeSpan animTime = TimeSpan.FromSeconds(0.25);

        animatingShadow = fromShadow.Clone();
        animatingShadow.BeginAnimation(DropShadowEffect.ShadowDepthProperty, new DoubleAnimation(targetShadow.ShadowDepth, animTime));
        animatingShadow.BeginAnimation(DropShadowEffect.ColorProperty,       new ColorAnimation (targetShadow.Color,       animTime));
        animatingShadow.BeginAnimation(DropShadowEffect.OpacityProperty,     new DoubleAnimation(targetShadow.Opacity     ,animTime));
        animatingShadow.BeginAnimation(DropShadowEffect.BlurRadiusProperty,  new DoubleAnimation(targetShadow.BlurRadius,  animTime));
        Effect = animatingShadow;                                                                                         
    }

    protected virtual void Gate_MouseDown(object sender, MouseButtonEventArgs e)
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
    protected virtual void Gate_MouseUp(object sender, MouseButtonEventArgs e) => Dragger?.DragEnd();
    protected virtual void Gate_MouseMove(object sender, MouseEventArgs e) => Dragger?.DragMove(e);
    protected virtual void Grid_Loaded(object sender, RoutedEventArgs e)
    {
        // Subscribe to mouse move, that way the mouse move will fire even if the cursor goes out of bounds, when dragging
        if (Window.GetWindow(this) is MainWindow mw)
            if (mw.MainGrid != null)
                mw.MainGrid.MouseMove += Gate_MouseMove;
    }
}//113, 83