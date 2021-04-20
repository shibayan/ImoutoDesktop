using System;
using System.IO;

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
                    OnPropertyChanged(nameof(Topmost));
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
                    OnPropertyChanged(nameof(UserName));
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
                using (var fs = File.Open(path, FileMode.Open))
                {
                    Default = Serializer<Settings>.Deserialize(fs);
                }
            }
            catch
            {
                Default = new Settings
                {
                    PortNumber = 1024,
                    Topmost = false,
                    UserName = "お兄ちゃん",
                    Honorific = string.Empty,
                    AutoDetectDirectoryType = false,
                    AllowImoutoAllOperation = false,
                    ShowFileList = false
                };
            }
        }

        public static void Save(string path)
        {
            using (var fs = File.Open(path, FileMode.Create))
            {
                Serializer<Settings>.Serialize(fs, Default);
            }
        }
    }
}
