namespace LogicLab;

public partial class LogicGate : LogicComponent
{
    public LogicGate() => InitializeComponent();

    protected override void Grid_Loaded(object sender, System.Windows.RoutedEventArgs e) => base.Grid_Loaded(sender, e);
    protected override void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) => base.Grid_MouseDown(sender, e);
    protected override void Grid_MouseMove(object sender, System.Windows.Input.MouseEventArgs e) => base.Grid_MouseMove(sender, e);
    protected override void Grid_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e) => base.Grid_MouseUp(sender, e);
}
