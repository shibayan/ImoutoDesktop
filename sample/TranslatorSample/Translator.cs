using System.Text;

using ImoutoDesktop.Commands;

namespace TranslatorSample
{
    public class Translator : ITranslate
    {
        #region ITranslate メンバ

        public Priority Priority
        {
            get { return Priority.Normal; }
        }

        public void Initialize(string path)
        {
            // サンプルでは何もしない
        }

        public void Uninitialize()
        {
            // サンプルでは何もしない
        }

        public bool Execute(string input, out string result)
        {
            var sb = new StringBuilder(input);

            // 。を音符に変えるだけ
            sb.Replace("。", "♪");

            result = sb.ToString();

            // true を返すと変更結果が反映される
            return true;
        }

        #endregion
    }
}
