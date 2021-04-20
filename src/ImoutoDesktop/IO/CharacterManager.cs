using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ImoutoDesktop.IO
{
    public static class CharacterManager
    {
        public static string RootDirectory { get; private set; }

        private static readonly Dictionary<Guid, Character> _characters = new Dictionary<Guid, Character>();

        public static Dictionary<Guid, Character> Characters
        {
            get { return CharacterManager._characters; }
        }

        public static bool TryGetCharacter(Guid id, out Character character)
        {
            return _characters.TryGetValue(id, out character);
        }

        public static void Rebuild(string searchDirectory)
        {
            // ルートを保存する
            RootDirectory = searchDirectory;
            // ディクショナリを削除する
            _characters.Clear();
            // ディレクトリを辿る
            foreach (var directory in Directory.GetDirectories(searchDirectory))
            {
                var path = Path.Combine(directory, "character.xml");
                if (!File.Exists(path))
                {
                    // 定義ファイルがないので無効
                    continue;
                }
                using (var stream = File.Open(path, FileMode.Open))
                {
                    var character = Serializer<Character>.Deserialize(stream);
                    character.Directory = directory;
                    _characters.Add(character.ID, character);
                }
            }
        }
    }
}
