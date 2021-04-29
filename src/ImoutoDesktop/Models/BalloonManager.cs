using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ImoutoDesktop.Models
{
    public static class BalloonManager
    {
        public static string RootDirectory { get; private set; }

        public static Dictionary<Guid, Balloon> Balloons { get; } = new();

        public static Balloon GetBalloon(Guid id)
        {
            return Balloons.TryGetValue(id, out var balloon) ? balloon : Balloons.First().Value;
        }

        public static bool TryGetBalloon(Guid id, out Balloon balloon)
        {
            return Balloons.TryGetValue(id, out balloon);
        }

        public static void Rebuild(string searchDirectory)
        {
            // ルートを保存する
            RootDirectory = searchDirectory;

            // ディクショナリを削除する
            Balloons.Clear();

            // ディレクトリを辿る
            foreach (var directory in Directory.GetDirectories(searchDirectory))
            {
                var path = Path.Combine(directory, "balloon.yml");

                if (!File.Exists(path))
                {
                    // 定義ファイルがないので無効
                    continue;
                }

                var balloon = Balloon.LoadFrom(path);

                Balloons.Add(balloon.Id, balloon);
            }
        }
    }
}
