using System;

using ImoutoDesktop.Commands;

namespace PluginSample
{
    public class SampleCommand : ICommand
    {
        #region ICommand メンバ

        /// <summary>
        /// コマンドの実行優先度を指定する
        /// </summary>
        public Priority Priority
        {
            get { return Priority.Lowest; }
        }

        public string EventID { get; set; }

        public string[] Parameters { get; set; }

        public void Initialize(string path)
        {
            // path にはプラグインのディレクトリが入ってくる
        }

        public void Uninitialize()
        {
            // 本体が終了時に呼ばれる
        }

        public bool IsExecute(string input)
        {
            // true を返すと呼び出し候補に入る、false だと絶対に呼び出されない
            return input.Contains("テスト");
        }

        public bool PreExecute(string input)
        {
            // 実行前の前処理を行う
            // ここで EventID と Parameters に値を入れておくと
            // "On" + EventID というイベントが Parameters をパラメータとして呼び出される
            // 実行が可能なら true を、出来ないなら false を返す
            // false を返すと "On" + EventID + "Failure" というイベントが呼び出される
            // EventID を省略するとこのクラス名が使われる
            return true;
        }

        public bool Execute(string input, out string result)
        {
            // 実行が成功なら true を、失敗なら false を返す
            // false を返すと "On" + EventID + "Failure" というイベントが呼び出される
            // result には実行結果のメッセージなどを格納することが出来る
            result = input + "\n" + DateTime.Now.ToString();
            return true;
        }

        #endregion
    }
}
