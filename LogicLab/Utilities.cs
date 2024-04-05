using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LogicLab;

public static class Utilities
{
    public static ImageBrush GetImage(string path) => new(new BitmapImage(new Uri($"Images/{path}.png", UriKind.Relative)));
}
