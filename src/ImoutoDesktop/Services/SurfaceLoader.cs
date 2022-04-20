using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;

using ImoutoDesktop.Models;

namespace ImoutoDesktop.Services;

public class SurfaceLoader
{
    public SurfaceLoader(string directory)
    {
        _rootDirectory = directory;

        RebuildTable();
    }

    private readonly string _rootDirectory;
    private readonly Dictionary<int, Surface> _surfaceTable = new();

    public void RebuildTable()
    {
        _surfaceTable.Clear();

        var regex = new Regex(@"surface([0-9]+).png$", RegexOptions.IgnoreCase);

        foreach (var file in Directory.GetFiles(_rootDirectory, "surface*.png"))
        {
            var match = regex.Match(file);

            if (match.Success)
            {
                var id = int.Parse(match.Groups[1].Value);

                _surfaceTable.Add(id, new Surface(id, Path.GetFileName(file)));
            }
        }
    }

    public Surface Load(int id)
    {
        if (!_surfaceTable.TryGetValue(id, out var surface))
        {
            return null;
        }

        surface.Image ??= new BitmapImage(new Uri(Path.Combine(_rootDirectory, surface.FileName)));

        return surface;
    }
}
