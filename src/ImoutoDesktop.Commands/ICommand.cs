using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        // コマンド初期化
        void Initialize(string path);
        // コマンド終了
        void Uninitialize();
        // コマンドが実行可能か
        bool IsExecute(string input);
        // 実行前準備
        bool PreExecute(string input);
        // コマンド実行
        bool Execute(string input, out string result);
    }
}
