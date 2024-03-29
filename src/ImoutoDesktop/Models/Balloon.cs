﻿using System;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;

namespace ImoutoDesktop.Models;

public class Balloon
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string ImoutoColor { get; set; } = "#000000";

    public string UserColor { get; set; } = "#000000";

    public BitmapImage BaseImage => new(new Uri(Path.Combine(Directory, "balloon.png")));

    public BitmapImage ArrowUpImage => new(new Uri(Path.Combine(Directory, "arrow0.png")));

    public BitmapImage ArrowDownImage => new(new Uri(Path.Combine(Directory, "arrow1.png")));

    public string Directory { get; set; }

    public override int GetHashCode() => Id.GetHashCode();

    public static Balloon LoadFrom(string path)
    {
        using var reader = new StreamReader(path, Encoding.UTF8);

        var balloon = Serializer.Deserialize<Balloon>(reader);

        balloon.Directory = Path.GetDirectoryName(path);

        return balloon;
    }
}
