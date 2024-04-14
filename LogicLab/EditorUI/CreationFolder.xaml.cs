using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace LogicLab.EditorUI;

// Austin (entire class)
public partial class CreationFolder : UserControl
{
    public static readonly DependencyProperty CustomTextProperty =
    DependencyProperty.Register("FolderName", typeof(string), typeof(CreationFolder),
        new PropertyMetadata("Default Text", OnFolderNameChanged));

    public string FolderName
    {
        get => (string)GetValue(CustomTextProperty);
        set => SetValue(CustomTextProperty, value);
    }
    public bool IsOpen
    {
        get => isOpen;
        set
        {
            isOpen = value;
            
            if (!IsOpen)
            {
                Height = FolderArrow.Height + 5;
                ItemPanel.Visibility = Visibility.Collapsed;

                if (parentFolder != null)
                {
                    parentFolder.ItemPanel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    parentFolder.ItemPanel.Arrange(new Rect(0, 0, ItemPanel.DesiredSize.Width, ItemPanel.DesiredSize.Height));
                    parentFolder.Height = parentFolder.ItemPanel.ActualHeight + 20;
                }

            }
            if (IsOpen)
            {
                ItemPanel.Visibility = Visibility.Visible;
                ItemPanel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                ItemPanel.Arrange(new Rect(0, 0, ItemPanel.DesiredSize.Width, ItemPanel.DesiredSize.Height));
                Height = ItemPanel.ActualHeight + 20;

                if (parentFolder != null)
                    parentFolder.Height += ItemPanel.ActualHeight;
            }
        }
    }

    private readonly SolidColorBrush transparentBrush = new(Color.FromArgb(0, 0, 0, 0));
    private readonly SolidColorBrush highlightBrush   = new(Color.FromArgb(255, 40, 40, 40));
    private readonly SolidColorBrush arrowHighlight   = new(Color.FromRgb(0, 100, 150));
    private readonly SolidColorBrush arrowDefault;
    private bool isOpen;
    private readonly CreationFolder? parentFolder;

    public CreationFolder()
    {
        InitializeComponent();
        BitmapImage bitmapImage = new(new Uri($"Images/Buffer Gate.png", UriKind.Relative));
        FolderArrow.OpacityMask = new ImageBrush(bitmapImage);

        FolderLabel.MouseEnter += Highlight_MouseEnter;
        FolderLabel.MouseLeave += Highlight_MouseLeave;
        FolderLabel.MouseDown  += FolderArrow_MouseDown;
        Highlight  .MouseDown  += FolderArrow_MouseDown;
        FolderArrow.MouseEnter += Highlight_MouseEnter;
        FolderArrow.MouseLeave += Highlight_MouseLeave;
        arrowDefault = (SolidColorBrush)FolderArrow.Fill;
        IsOpen = false;
    }
    public CreationFolder(CreationFolder parentFolder) : this() => this.parentFolder = parentFolder;

    public void AddItem(CreationMenu creationMenu, MainWindow mainWindow, Func<LogicComponent> buildLogicComponent, 
        string label, ImageBrush? image = null, bool showNot = false) 
    {
        CreationItem item = new(creationMenu, mainWindow, label, buildLogicComponent, image, showNot);
        ItemPanel.Children.Add(item);
    }
    public void AddFolder(CreationFolder folder) => ItemPanel.Children.Add(folder);

    private static void OnFolderNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is CreationFolder folder && e.NewValue is string newValue)
            folder.FolderLabel.Content = newValue;
    }

    private void Highlight_MouseEnter  (object sender, MouseEventArgs e) => Highlight  .Fill = highlightBrush;
    private void Highlight_MouseLeave  (object sender, MouseEventArgs e) => Highlight  .Fill = transparentBrush;
    private void FolderArrow_MouseLeave(object sender, MouseEventArgs e) => FolderArrow.Fill = arrowDefault;
    private void FolderArrow_MouseEnter(object sender, MouseEventArgs e) => FolderArrow.Fill = arrowHighlight;

    public void FolderArrow_MouseDown(object sender, MouseButtonEventArgs e)
    {
        IsOpen = !IsOpen;

        DoubleAnimation animation = new()
        {
            From     = IsOpen ? 0 : 90,
            To       = IsOpen ? 90 : 0,  
            Duration = TimeSpan.FromSeconds(0.1)  
        };

        RotateTransform rotateTransform = new()
        {
            CenterX = FolderArrow.Width / 2,
            CenterY = FolderArrow.Height / 2
        };
        FolderArrow.RenderTransform = rotateTransform;
        rotateTransform.BeginAnimation(RotateTransform.AngleProperty, animation);
    }
}
