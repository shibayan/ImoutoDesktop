using System;
using System.Xml.Serialization;

namespace ImoutoDesktop.IO
{
    [Serializable]
    public class Character : PropertyChangedBase
    {
        public Character()
        {
            Age = 10;
            TsundereLevel = 4;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public int TsundereLevel { get; set; }

        [XmlIgnore]
        public bool CanSelect { get; set; }

        [XmlIgnore]
        public string Directory { get; set; }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
