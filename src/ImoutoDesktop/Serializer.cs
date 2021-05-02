using System.IO;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ImoutoDesktop
{
    public static class Serializer
    {
        static Serializer()
        {
            _serializer = new SerializerBuilder()
                          .WithNamingConvention(CamelCaseNamingConvention.Instance)
                          .Build();

            _deserializer = new DeserializerBuilder()
                            .WithNamingConvention(CamelCaseNamingConvention.Instance)
                            .Build();
        }

        private static readonly ISerializer _serializer;
        private static readonly IDeserializer _deserializer;

        public static T Deserialize<T>(TextReader textReader)
        {
            try
            {
                return _deserializer.Deserialize<T>(textReader);
            }
            catch
            {
                return default;
            }
        }

        public static void Serialize<T>(TextWriter textWriter, T obj)
        {
            if (obj == null)
            {
                return;
            }

            _serializer.Serialize(textWriter, obj);
        }
    }
}
