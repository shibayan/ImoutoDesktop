using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ImoutoDesktop
{
    public static class Serializer<T> where T : new()
    {
        private static readonly XmlSerializer xs = new XmlSerializer(typeof(T));

        public static T Deserialize(Stream stream)
        {
            try
            {
                return (T)xs.Deserialize(stream);
            }
            catch
            {
                return new T();
            }
        }

        public static T Deserialize(string path)
        {
            try
            {
                using (var stream = File.Open(path, FileMode.Open))
                {
                    return (T)xs.Deserialize(stream);
                }
            }
            catch
            {
                return new T();
            }
        }

        public static void Serialize(Stream stream, T o)
        {
            if (o == null)
            {
                return;
            }
            xs.Serialize(stream, o);
        }

        public static void Serialize(string path, T o)
        {
            if (o == null)
            {
                return;
            }
            using (var stream = File.Open(path, FileMode.Create))
            {
                xs.Serialize(stream, o);
            }
        }
    }
}
