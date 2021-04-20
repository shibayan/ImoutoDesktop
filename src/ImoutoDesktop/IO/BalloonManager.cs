using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace ImoutoDesktop.IO
{
    public static class BalloonManager
    {
        public static string RootDirectory { get; private set; }

        private static readonly Dictionary<Guid, Balloon> _balloons = new Dictionary<Guid, Balloon>();

        public static Dictionary<Guid, Balloon> Balloons
        {
            get { return BalloonManager._balloons; }
        }

        public static Balloon GetBalloon(Guid id)
        {
            Balloon balloon;
            if (_balloons.TryGetValue(id, out balloon))
            {
                return balloon;
            }
            return _balloons.ElementAt(0).Value;
        }

        public static bool TryGetBalloon(Guid id, out Balloon balloon)
        {
            return _balloons.TryGetValue(id, out balloon);
        }

        public static void Rebuild(string searchDirectory)
        {
            // ルートを保存する
            RootDirectory = searchDirectory;
            // ディクショナリを削除する
            _balloons.Clear();
            // ディレクトリを辿る
            foreach (var directory in Directory.GetDirectories(searchDirectory))
            {
                var path = Path.Combine(directory, "balloon.xml");
                if (!File.Exists(path))
                {
                    // 定義ファイルがないので無効
                    continue;
                }
                using (var stream = File.Open(path, FileMode.Open))
                {
                    var balloon = Serializer<Balloon>.Deserialize(stream);
                    balloon.Directory = directory;
                    _balloons.Add(balloon.ID, balloon);
                }
            }
        }
    }
}
