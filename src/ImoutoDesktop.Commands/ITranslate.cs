namespace ImoutoDesktop.Commands
{
    public interface ITranslate
    {
        // コマンドの優先度
        Priority Priority { get; }
        // コマンド初期化
        void Initialize(string path);
        // コマンド終了
        void Uninitialize();
        // コマンド実行
        bool Execute(string input, out string result);
    }
}
