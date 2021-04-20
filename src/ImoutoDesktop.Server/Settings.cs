using System.IO;
using System.Xml.Serialization;

namespace ImoutoDesktop.Server
{
    public class Settings
    {
        public int PortNumber { get; set; }

        public string Password { get; set; }

        public static Settings Default { get; private set; }

        private static readonly XmlSerializer xs = new XmlSerializer(typeof(Settings));

        public static void Load(string path)
        {
            try
            {
                using (var fs = File.Open(path, FileMode.Open))
                {
                    Default = (Settings)xs.Deserialize(fs);
                }
            }
            catch
            {
                Default = new Settings();
                Default.PortNumber = 1024;
                Default.Password = string.Empty;
            }
        }

        public static void Save(string path)
        {
            using (var fs = File.Open(path, FileMode.Create))
            {
                xs.Serialize(fs, Default);
            }
        }
    }
}
