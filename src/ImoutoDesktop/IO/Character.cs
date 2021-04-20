using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using ImoutoDesktop;

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

        private Guid _id;

        public Guid ID
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged("ID");
                }
            }
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

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

        public int Age { get; set; }

        public int TsundereLevel { get; set; }

        private string _directory;

        [XmlIgnore]
        public string Directory
        {
            get { return _directory; }
            set
            {
                if (_directory != value)
                {
                    _directory = value;
                    OnPropertyChanged("Directory");
                }
            }
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }
    }
}
