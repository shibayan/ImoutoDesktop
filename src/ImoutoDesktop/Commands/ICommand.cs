namespace ImoutoDesktop.Commands
{
    public interface ICommand
    {
        // コマンドの優先度
        Priority Priority { get; }
        // イベント名
        string EventID { get; set; }
        // イベントパラメータ
        string[] Parameters { get; set; }
        // コマンドが実行可能か
        bool IsExecute(string input);
        // 実行前準備
        bool PreExecute(string input);
        // コマンド実行
        bool Execute(string input, out string result);
    }
}
