using System;
using System.IO;
using System.Xml.Serialization;

namespace ImoutoDesktop.IO
{
    [Serializable]
    public class Balloon : PropertyChangedBase
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

        [XmlIgnore]
        public bool CanSelect { get; set; }

        [XmlIgnore]
        public string Image => Path.Combine(Directory, "balloon.png");

        [XmlIgnore]
        public string ArrowUpImage => Path.Combine(Directory, "arrow0.png");

        [XmlIgnore]
        public string ArrowDownImage => Path.Combine(Directory, "arrow1.png");

        [XmlIgnore]
        public string Directory { get; set; }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
