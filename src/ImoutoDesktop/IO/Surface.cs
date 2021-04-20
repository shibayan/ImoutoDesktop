using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace ImoutoDesktop.IO
{
    public class Surface : PropertyChangedBase
    {
        public Surface(int id, string fileName)
        {
            _id = id;
            _fileName = fileName;
        }

        private readonly int _id;

        public int ID
        {
            get { return _id; }
        }

        private readonly string _fileName;

        public string FileName
        {
            get { return _fileName; }
        }

        private IEnumerable<Animation> _animations;

        public IEnumerable<Animation> Animations
        {
            get { return _animations; }
            set
            {
                if (_animations != value)
                {
                    _animations = value;
                    OnPropertyChanged(nameof(Animations));
                }
            }
        }

        private WeakReference _image;

        public ImageSource Image
        {
            get { return (ImageSource)_image?.Target; }
            set
            {
                _image = new WeakReference(value);
                OnPropertyChanged(nameof(Image));
            }
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }
    }
}
