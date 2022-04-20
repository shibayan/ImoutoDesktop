using System.Windows.Media.Imaging;

namespace ImoutoDesktop.Models;

public class Surface
{
    public Surface(int id, string fileName)
    {
        Id = id;
        FileName = fileName;
    }

    public int Id { get; }

    public string FileName { get; }

    public BitmapImage Image { get; set; }

    public override int GetHashCode() => Id.GetHashCode();
}
