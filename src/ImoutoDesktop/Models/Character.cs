using System.IO;
using System.Text;

namespace ImoutoDesktop.Models;

public class Character
{
    public required string Id { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }

    public int Age { get; set; } = 10;

    public int TsundereLevel { get; set; } = 4;

    public string? Directory { get; set; }

    public override int GetHashCode() => Id.GetHashCode();

    public static Character? LoadFrom(string path)
    {
        using var reader = new StreamReader(path, Encoding.UTF8);

        var character = Serializer.Deserialize<Character>(reader);

        if (character is not null)
        {
            character.Directory = Path.GetDirectoryName(path);
        }

        return character;
    }
}
