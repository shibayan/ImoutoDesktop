using System;
using System.IO;
using System.Windows;

namespace ImoutoDesktop.Models
{
    public class Profile
    {
        public Guid LastBalloon { get; set; }

        public Point BalloonOffset { get; set; }

        public int Age { get; set; }

        public int TsundereLevel { get; set; }

        public void SaveTo(string path)
        {
            using var writer = new StreamWriter(path);

            Serializer.Serialize(writer, this);
        }

        public static Profile LoadFrom(string path)
        {
            using var reader = new StreamReader(path);

            return Serializer.Deserialize<Profile>(reader);
        }
    }
}
