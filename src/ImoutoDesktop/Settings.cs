using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ImoutoDesktop
{
    public class Settings : PropertyChangedBase
    {
        public string ServerAddress { get; set; }

        public int PortNumber { get; set; }

        public string Password { get; set; }

        private bool _topmost;

        public bool Topmost
        {
            get { return _topmost; }
            set
            {
                if (_topmost != value)
                {
                    _topmost = value;
                    OnPropertyChanged("Topmost");
                }
            }
        }

        private string _userName;

        public string UserName
        {
            get { return _userName; }
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    OnPropertyChanged("UserName");
                }
            }
        }

        public string Honorific { get; set; }

        public bool AutoDetectDirectoryType { get; set; }

        public bool AllowImoutoAllOperation { get; set; }

        public bool ShowFileList { get; set; }

        public Guid? LastCharacter { get; set; }

        public static Settings Default { get; private set; }

        public static void Load(string path)
        {
            try
            {
                using (FileStream fs = File.Open(path, FileMode.Open))
                {
                    Default = Serializer<Settings>.Deserialize(fs);
                }
            }
            catch
            {
                Default = new Settings();
                Default.PortNumber = 1024;
                Default.Topmost = false;
                Default.UserName = "お兄ちゃん";
                Default.Honorific = string.Empty;
                Default.AutoDetectDirectoryType = false;
                Default.AllowImoutoAllOperation = false;
                Default.ShowFileList = false;
            }
        }

        public static void Save(string path)
        {
            using (FileStream fs = File.Open(path, FileMode.Create))
            {
                Serializer<Settings>.Serialize(fs, Default);
            }
        }
    }
}
