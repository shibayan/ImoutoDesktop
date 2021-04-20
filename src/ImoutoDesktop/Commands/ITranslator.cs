namespace ImoutoDesktop.Commands
{
    public interface ITranslator
    {
        // コマンドの優先度
        Priority Priority { get; }
        // コマンド実行
        string Translate(string input);
    }
}
