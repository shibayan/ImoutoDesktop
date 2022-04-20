using System.IO;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ImoutoDesktop;

public static class Serializer
{
    static Serializer()
    {
    }

    private static readonly ISerializer s_serializer = new SerializerBuilder()
                                                       .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                                       .Build();
    private static readonly IDeserializer s_deserializer = new DeserializerBuilder()
                                                           .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                                           .Build();

    public static T Deserialize<T>(TextReader textReader)
    {
        try
        {
            return s_deserializer.Deserialize<T>(textReader);
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

        s_serializer.Serialize(textWriter, obj);
    }
}
