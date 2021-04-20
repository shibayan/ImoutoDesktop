using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
                using (FileStream fs = File.Open(path, FileMode.Open))
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
            using (FileStream fs = File.Open(path, FileMode.Create))
            {
                xs.Serialize(fs, Default);
            }
        }
    }
}
