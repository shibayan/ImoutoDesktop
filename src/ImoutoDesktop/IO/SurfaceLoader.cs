using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;

namespace ImoutoDesktop.IO
{
    public class SurfaceLoader
    {
        public SurfaceLoader(string directory)
        {
            RebuildTable(directory);
        }

        public string RootDirectory { get; set; }

        private readonly Dictionary<int, Surface> _surfaceTable = new Dictionary<int, Surface>();

        public void RebuildTable()
        {
            _surfaceTable.Clear();
            var regex = new Regex(@"surface([0-9]+).png$", RegexOptions.IgnoreCase);
            foreach (var file in Directory.GetFiles(RootDirectory, "surface*.png"))
            {
                var match = regex.Match(file);
                if (match.Success)
                {
                    var id = int.Parse(match.Groups[1].Value);
                    _surfaceTable.Add(id, new Surface(id, Path.GetFileName(file)));
                }
            }
        }

        public void RebuildTable(string directory)
        {
            RootDirectory = directory;
            // ID -> ファイル名のテーブルを作成する
            RebuildTable();
        }

        public Surface Load(int id)
        {
            Surface surface;
            if (_surfaceTable.TryGetValue(id, out surface))
            {
                if (surface.Image == null)
                {
                    var image = new BitmapImage(new Uri(Path.Combine(RootDirectory, surface.FileName)));
                    surface.Image = image;
                }
                return surface;
            }
            return null;
        }
    }
}
