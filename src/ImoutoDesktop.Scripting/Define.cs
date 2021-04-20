using System.Text.RegularExpressions;

namespace ImoutoDesktop.Scripting
{
    /// <summary>
    /// プリプロセッサでの置き換え文字列を保持します。
    /// </summary>
    class Define
    {
        /// <summary>
        /// 検索、置換文字列のペアを作成します。
        /// </summary>
        /// <param name="before">検索文字列。</param>
        /// <param name="after">置き換え文字列。</param>
        public Define(string before, string after)
        {
            this.before = new Regex(before);
            this.after = after;
        }

        private Regex before;

        /// <summary>
        /// 検索正規表現を取得します。
        /// </summary>
        public Regex Before
        {
            get { return before; }
        }
        private string after;

        /// <summary>
        /// 置き換え文字列を取得します。
        /// </summary>
        public string After
        {
            get { return after; }
        }
    }
}
