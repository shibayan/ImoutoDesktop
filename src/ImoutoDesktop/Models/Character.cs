using System.IO;
using System.Text;

namespace ImoutoDesktop.Models
{
    public class Character
    {
        public Character()
        {
            Age = 10;
            TsundereLevel = 4;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Age { get; set; }

        public int TsundereLevel { get; set; }

        public bool CanSelect { get; set; }

        public string Directory { get; set; }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static Character LoadFrom(string path)
        {
            using var reader = new StreamReader(path, Encoding.UTF8);

            var character = Serializer.Deserialize<Character>(reader);

            character.Directory = Path.GetDirectoryName(path);

            return character;
        }
    }
}
