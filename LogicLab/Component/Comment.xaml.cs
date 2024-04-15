using System.Drawing;
using System.Windows;
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
        bool isMouseDown;
        

        this.MainWindow().DebugLabel.Content += CommentBox.ActualHeight.ToString() + ", ";
        //this.MainWindow().DebugLabel.Content += e.GetPosition(this).ToString() + ", ";

        // Top Left Corner
        if (pos.X < cornerLength && pos.Y < cornerLength)
        {
          //  System.Drawing.Cursor cursor = System.Windows.Forms.Cursors.NE;
            Cursor = Cursors.ScrollNW;
            //move the draging code into each bar's conditional as an if MouseDown Event




            //make sure to ask austin how to use the MouseDown Event, How to fix resizing to be only in the dragged direction and how to make ActualHeight edditable




        }
        // Top Right Corner
        else if (pos.X > CommentSprite.ActualWidth - cornerLength && pos.Y < cornerLength)
        {
            Cursor = Cursors.ScrollNE;
            //move the draging code into each bar's conditional as an if MouseDown Event
        }
        // Bottom Left Corner
        else if (pos.X < cornerLength && pos.Y > CommentSprite.ActualHeight - cornerLength)
        {
            Cursor = Cursors.ScrollSW;
            //move the draging code into each bar's conditional as an if MouseDown Event
        }
        // Bottom Right Corner
        else if (pos.X > CommentSprite.ActualWidth - cornerLength && pos.Y > CommentSprite.ActualHeight - cornerLength)
        {
            Cursor = Cursors.ScrollSE;
            //move the draging code into each bar's conditional as an if MouseDown Event
        }

        // Top Bar
        else if (pos.Y < sideBarLength-5)
        {
            Cursor = Cursors.SizeNS;
            //move the draging code into each bar's conditional as an if MouseDown Event

        }
        else if (pos.Y < titleBarLength)
        {
            Cursor = Cursors.SizeAll;
            //move the draging code into each bar's conditional as an if MouseDown Event
        }
        // Left Bar
        else if (pos.X < sideBarLength)
        {
            Cursor = Cursors.SizeWE;
            //move the draging code into each bar's conditional as an if MouseDown Event
        }
        // Right Bar
        else if (pos.X > CommentSprite.ActualWidth - sideBarLength)
        {
            Cursor = Cursors.SizeWE;
            //move the draging code into each bar's conditional as an if MouseDown Event
        }
        // Bottom Bar
        else if (pos.Y > CommentSprite.ActualHeight - sideBarLength)
        {
            Cursor = Cursors.SizeNS;
            //move the draging code into each bar's conditional as an if MouseDown Event
        }
        else
        {
            Cursor = Cursors.Arrow;
            //move the draging code into each bar's conditional as an if MouseDown Event
        }
        //void DragStart(MouseEventArgs e)
        //{

        //isMouseDown = true;


        //}

        // making height change depending on cursor type
        //if (Cursor == Cursors.SizeNS)
        //{
        //ActualHeight = ActualHeight + ;
        //}
        //else if (Cursor == Cursors.SizeWE)
        //{
        //  ActualWidth = ActualWidth + ;
        //}
        //else if (Cursor == Cursors.ScrollNW && isMouseDown == true)
        //{
        // ActualHeight = ActualHeight + ;
        // ActualWidth = ActualWidth + ;
        //}
    }

    //move and drag the comments different places
}
