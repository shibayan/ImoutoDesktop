using System;
using System.IO;

namespace ImoutoDesktop.Models
{
    public class Balloon
    {
        public Balloon()
        {
            ImoutoColor = "#000000";
            UserColor = "#000000";
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string ImoutoColor { get; set; }

        public string UserColor { get; set; }

        public bool CanSelect { get; set; }

        public string Image => Path.Combine(Directory, "balloon.png");

        public string ArrowUpImage => Path.Combine(Directory, "arrow0.png");

        public string ArrowDownImage => Path.Combine(Directory, "arrow1.png");

        public string Directory { get; set; }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static Balloon LoadFrom(string path)
        {
            using var reader = new StreamReader(path);

            var balloon = Serializer.Deserialize<Balloon>(reader);

            balloon.Directory = Path.GetDirectoryName(path);

            return balloon;
        }
    }
}
