using System.IO;
using System.Text;

namespace ImoutoDesktop.Models
{
    public class Settings : PropertyChangedBase
    {
        public string ServerAddress { get; set; }

        public int PortNumber { get; set; }

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

        public string LastCharacter { get; set; }

        public static Settings Default { get; private set; }

        public static void Load(string path)
        {
            try
            {
                using var reader = new StreamReader(path, Encoding.UTF8);

                Default = Serializer.Deserialize<Settings>(reader);
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
            using var writer = new StreamWriter(path);

            Serializer.Serialize(writer, Default);
        }
    }
}
