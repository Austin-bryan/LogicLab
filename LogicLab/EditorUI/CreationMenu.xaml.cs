using LogicLab.Component;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace LogicLab.EditorUI;

// Austin
public partial class CreationMenu : UserControl
{
    private CreationFolder directGates;
    private CreationFolder invertedGates;
    private static List<Variant<bool, List<bool>>>? creationMenuStatus;

    public CreationMenu() => InitializeComponent();
    private bool isPinned = false;
    
    public void Remove()
    {
        if (isPinned)
            return;
        UpdateCreationMenuStatus();
        ((Grid)Parent).Children.Remove(this);
    }

    private void UpdateCreationMenuStatus()
    {
        creationMenuStatus = [];
        List<bool> logicGateStatus = [];

        logicGateStatus.Add(directGates.IsOpen);
        logicGateStatus.Add(invertedGates.IsOpen);

        creationMenuStatus.Add(new Variant<bool, List<bool>>(LogicFolder.IsOpen));
        creationMenuStatus.Add(new Variant<bool, List<bool>>(logicGateStatus));
        creationMenuStatus.Add(new Variant<bool, List<bool>>(OutputFolder.IsOpen));
        creationMenuStatus.Add(new Variant<bool, List<bool>>(InputFolder.IsOpen));
    }

    private void Grid_Loaded(object sender, RoutedEventArgs e)
    {
        directGates = new(LogicFolder);
        directGates.FolderName = "Direct Gates";
        directGates.AddItem(this, this.MainWindow(), () => new LogicGate(ELogicGate.Buffer), "Buffer Gate", ELogicGate.Buffer.GetImage());
        directGates.AddItem(this, this.MainWindow(), () => new LogicGate(ELogicGate.OR), "OR Gate", ELogicGate.OR.GetImage());
        directGates.AddItem(this, this.MainWindow(), () => new LogicGate(ELogicGate.AND), "AND Gate", ELogicGate.AND.GetImage());
        directGates.AddItem(this, this.MainWindow(), () => new LogicGate(ELogicGate.XOR), "XOR Gate", ELogicGate.XOR.GetImage());

        invertedGates = new(LogicFolder);
        invertedGates.FolderName = "Inverted Gates";
        invertedGates.AddItem(this, this.MainWindow(), () => new LogicGate(ELogicGate.NOT), "NOT Gate",   ELogicGate.Buffer.GetImage(), true);
        invertedGates.AddItem(this, this.MainWindow(), () => new LogicGate(ELogicGate.NOR), "NOR Gate",   ELogicGate.OR.GetImage(), true);
        invertedGates.AddItem(this, this.MainWindow(), () => new LogicGate(ELogicGate.NAND), "NAND Gate", ELogicGate.AND.GetImage(), true);
        invertedGates.AddItem(this, this.MainWindow(), () => new LogicGate(ELogicGate.XNOR), "XNOR Gate", ELogicGate.XOR.GetImage(), true);

        LogicFolder.AddFolder(directGates);
        LogicFolder.AddFolder(invertedGates);

        OutputFolder.AddItem(this, this.MainWindow(), () => new OutputToggle(),        "Toggle", Utilities.GetImage("OnOff"));
        OutputFolder.AddItem(this, this.MainWindow(), () => new OutputConstant(true),  "Constant On", Utilities.GetImage("On"));
        OutputFolder.AddItem(this, this.MainWindow(), () => new OutputConstant(false), "Constant Off", Utilities.GetImage("Off"));

        InputFolder.AddItem(this, this.MainWindow(), () => new InputPixel(), "Pixel", Utilities.GetImage("Pixel"));
        InputFolder.AddItem(this, this.MainWindow(), () => new InputHexDisplay(), "4 Bit-Hex Display", Utilities.GetImage("4Bit Display"));

        RestoreStatus();
    }

    public void RestoreStatus()
    {
        if (creationMenuStatus == null)
            return;
        TryOpenFolder(creationMenuStatus[0].GetValue<bool>(), LogicFolder);
        TryOpenFolder(creationMenuStatus[1].GetValue<List<bool>>()[0], directGates);
        TryOpenFolder(creationMenuStatus[1].GetValue<List<bool>>()[1], invertedGates);
        TryOpenFolder(creationMenuStatus[2].GetValue<bool>(), OutputFolder);
        TryOpenFolder(creationMenuStatus[3].GetValue<bool>(), InputFolder);

        void TryOpenFolder(bool shouldOpen, CreationFolder folder)
        {
            if (shouldOpen)
                folder.FolderArrow_MouseDown(this, null);
        }
    }

    public void HideOutput() => FolderPanel.Children.Remove(OutputFolder);
    public void HideInput()  => FolderPanel.Children.Remove(InputFolder);


    private void PinSprite_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (PinSprite.RenderTransform as RotateTransform == null)
        {
            PinSprite.RenderTransform = new RotateTransform(0);
            PinSprite.RenderTransformOrigin = new Point(0.5, 0.5); // Rotate around the center
        }

        isPinned = !isPinned;

        DoubleAnimation rotateAnimation = new()
        {
            From           = isPinned ? 90 : 0,
            To             = isPinned ? 0 : 90,
            Duration       = TimeSpan.FromSeconds(0.125), 
            EasingFunction = new QuadraticEase()
        };

        PinSprite.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);
    }
}