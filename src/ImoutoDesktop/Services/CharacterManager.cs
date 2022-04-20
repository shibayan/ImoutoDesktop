using System.Collections.Generic;
using System.IO;

using ImoutoDesktop.Models;

namespace ImoutoDesktop.Services;

public static class CharacterManager
{
    public static string RootDirectory { get; private set; }

    public static Dictionary<string, Character> Characters { get; } = new();

    public static bool TryGetValue(string id, out Character character) => Characters.TryGetValue(id, out character);

    public static void Rebuild(string searchDirectory)
    {
        // ルートを保存する
        RootDirectory = searchDirectory;

        // ディクショナリを削除する
        Characters.Clear();

        // ディレクトリを辿る
        foreach (var directory in Directory.GetDirectories(searchDirectory))
        {
            var path = Path.Combine(directory, "character.yml");

            if (!File.Exists(path))
            {
                // 定義ファイルがないので無効
                continue;
            }

            var character = Character.LoadFrom(path);

            Characters.Add(character.Id, character);
        }
    }
}
