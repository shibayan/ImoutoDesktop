using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Windows;
using System.Windows.Media;

using ImoutoDesktop;

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

        public Guid ID { get; set; }

        public string Name { get; set; }

        public string ImoutoColor { get; set; }

        public string UserColor { get; set; }

        private bool _canSelect = true;

        [XmlIgnore]
        public bool CanSelect
        {
            get { return _canSelect; }
            set
            {
                _canSelect = value;
                OnPropertyChanged("CanSelect");
            }
        }

        [XmlIgnore]
        public string Image
        {
            get { return Path.Combine(Directory, "balloon.png"); }
        }

        [XmlIgnore]
        public string ArrowUpImage
        {
            get { return Path.Combine(Directory, "arrow0.png"); }
        }

        [XmlIgnore]
        public string ArrowDownImage
        {
            get { return Path.Combine(Directory, "arrow1.png"); }
        }

        [XmlIgnore]
        public string Directory { get; set; }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
    }
}