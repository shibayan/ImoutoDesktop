﻿using System.Windows.Media;

namespace ImoutoDesktop.IO
{
    public class Surface
    {
        public Surface(int id, string fileName)
        {
            Id = id;
            FileName = fileName;
        }

        public int Id { get; }

        public string FileName { get; }

        public ImageSource Image { get; set; }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
