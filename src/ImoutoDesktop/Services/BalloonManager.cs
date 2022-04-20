using System.Collections.Generic;
using System.IO;
using System.Linq;

using ImoutoDesktop.Models;

namespace ImoutoDesktop.Services;

public static class BalloonManager
{
    public static string RootDirectory { get; private set; }

    public static Dictionary<string, Balloon> Balloons { get; } = new();

    public static Balloon GetValueOrDefault(string id) => id != null && Balloons.TryGetValue(id, out var balloon) ? balloon : Balloons.First().Value;

    public static bool TryGetValue(string id, out Balloon balloon) => Balloons.TryGetValue(id, out balloon);

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
