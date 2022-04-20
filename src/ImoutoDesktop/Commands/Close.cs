namespace ImoutoDesktop.Commands;

public class Close : CommandBase
{
    public Close()
        : base("終了")
    {
    }

    public override Priority Priority => Priority.Lowest;
}
