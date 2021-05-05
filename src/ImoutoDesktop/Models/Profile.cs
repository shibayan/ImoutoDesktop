using System.IO;
using System.Text;
using System.Windows;

namespace ImoutoDesktop.Models
{
    public class Profile
    {
        public string LastBalloon { get; set; }

        public Point BalloonOffset { get; set; }

        public int? Age { get; set; }

        public int? TsundereLevel { get; set; }

        public void SaveTo(string path)
        {
            using var writer = new StreamWriter(path);

            Serializer.Serialize(writer, this);
        }

        public static Profile LoadFrom(string path)
        {
            try
            {
                using var reader = new StreamReader(path, Encoding.UTF8);

                return Serializer.Deserialize<Profile>(reader);
            }
            catch
            {
                return new Profile();
            }
        }
    }
}
