using System.Text.RegularExpressions;

namespace ImoutoDesktop.Scripting;

/// <summary>
/// プリプロセッサでの置き換え文字列を保持します。
/// </summary>
internal class Define
{
    /// <summary>
    /// 検索、置換文字列のペアを作成します。
    /// </summary>
    /// <param name="before">検索文字列。</param>
    /// <param name="after">置き換え文字列。</param>
    public Define(string before, string after)
    {
        Before = new Regex(before);
        After = after;
    }

    /// <summary>
    /// 検索正規表現を取得します。
    /// </summary>
    public Regex Before { get; }

    /// <summary>
    /// 置き換え文字列を取得します。
    /// </summary>
    public string After { get; }
}
