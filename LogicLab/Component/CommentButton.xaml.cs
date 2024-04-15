using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace LogicLab.Component;

/// <summary>
/// Interaction logic for ComentControl.xaml
/// Steven's Code
public partial class CommentButton : UserControl
{
    private TextBox currentTextBox;
    

    public CommentButton()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
    }

    private void Button_MouseDown(object sender, MouseButtonEventArgs e)
    {
        "Test".Show();
        Comment comment = new();
    }
}










//private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
//    {
//        Rect selectionRect = new(mouseDownPos, mouseUpPos);

//        if (isDragging && currentTextBox != null)
//        {
//            Canvas.SetLeft(currentTextBox, e.GetPosition(this).X - currentTextBox.Width / 2);
//            Canvas.SetTop(currentTextBox, e.GetPosition(this).Y - currentTextBox.Height / 2);
//        }
//    }

//}
