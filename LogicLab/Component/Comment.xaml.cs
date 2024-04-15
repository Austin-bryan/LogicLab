using System.Drawing;
using System.Windows.Controls;
using System.Windows.Input;

namespace LogicLab.Component;

/// Stevens Code
/// Interaction logic for Comment.xaml
/// </summary>
public partial class Comment : UserControl
{
    public Comment()
    {
        InitializeComponent();
    }

    private void Rectangle_MouseMove(object sender, MouseEventArgs e)
    {
        const int titleBarLength = 40;
        const int sideBarLength = 10;
        const int cornerLength = 30;
        var pos = e.GetPosition(this);

        this.MainWindow().DebugLabel.Content += CommentBox.ActualHeight.ToString() + ", ";
        //this.MainWindow().DebugLabel.Content += e.GetPosition(this).ToString() + ", ";

        // Top Left Corner
        if (pos.X < cornerLength && pos.Y < cornerLength)
        {
          //  System.Drawing.Cursor cursor = System.Windows.Forms.Cursors.NE;
            Cursor = Cursors.ScrollNW;
        }
        // Top Right Corner
        else if (pos.X > CommentSprite.ActualWidth - cornerLength && pos.Y < cornerLength)
        {
            Cursor = Cursors.ScrollNE;
        }
        // Bottom Left Corner
        else if (pos.X < cornerLength && pos.Y > CommentSprite.ActualHeight - cornerLength)
        {
            Cursor = Cursors.ScrollSW;
        }
        // Bottom Right Corner
        else if (pos.X > CommentSprite.ActualWidth - cornerLength && pos.Y > CommentSprite.ActualHeight - cornerLength)
        {
            Cursor = Cursors.ScrollSE;
        }

        // Top Bar
        else if (pos.Y < sideBarLength-5)
        {
            Cursor = Cursors.SizeNS;

        }
        else if (pos.Y < titleBarLength)
        {
            Cursor = Cursors.SizeAll;
        }
        // Left Bar
        else if (pos.X < sideBarLength)
        {
            Cursor = Cursors.SizeWE;
        }
        // Right Bar
        else if (pos.X > CommentSprite.ActualWidth - sideBarLength)
        {
            Cursor = Cursors.SizeWE;
        }
        // Bottom Bar
        else if (pos.Y > CommentSprite.ActualHeight - sideBarLength)
        {
            Cursor = Cursors.SizeNS;
        }
        else
        {
            Cursor = Cursors.Arrow;
        }

        // making height change depending on cursor type
        if (Cursor == Cursors.SizeNS)
        {
            ActualHeight = ActualHeight + ;
        }
        else if (Cursor == Cursors.SizeWE)
        {
            ActualWidth = ActualWidth + ;
        }
        else if (Cursor == Cursors.ScrollNW)
        {
            ActualHeight = ActualHeight + ;
            ActualWidth = ActualWidth + ;
        }
    }

    //move and drag the comments different places
}
