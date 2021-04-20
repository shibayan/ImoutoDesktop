namespace ImoutoDesktop.Scripting
{
    public class Token
    {
        public Token(TokenKind kind)
        {
            _kind = kind;
        }

        private Token(TokenKind kind, object value)
        {
            _kind = kind;
            _value = value;
        }

        private Token(TokenKind kind, object[] parameters)
        {
            _kind = kind;
            _parameters = parameters;
        }

        private readonly TokenKind _kind;

        public TokenKind Kind
        {
            get { return _kind; }
        }

        private object _value;

        /// <summary>
        /// タグが保持する値。
        /// </summary>
        public object Value
        {
            get { return _value; }
        }

        private object[] _parameters;

        /// <summary>
        /// タグの引数。
        /// </summary>
        public object[] Parameters
        {
            get { return _parameters; }
        }

        #region Token のファクトリ

        public static Token Text(string text)
        {
            return new Token(TokenKind.Text, text);
        }

        public static Token LineBreak()
        {
            return new Token(TokenKind.LineBreak);
        }

        public static Token Font(FontOperation operation, object value)
        {
            return new Token(TokenKind.Font, new object[] { operation, value });
        }

        public static Token Clear()
        {
            return new Token(TokenKind.Clear);
        }

        public static Token Surface(int id)
        {
            return new Token(TokenKind.Surface, id);
        }

        public static Token Sleep(int millisecond)
        {
            return new Token(TokenKind.Sleep, millisecond);
        }

        public static Token BringToFront()
        {
            return new Token(TokenKind.BringToFront);
        }

        public static Token Exit()
        {
            return new Token(TokenKind.Exit);
        }

        public static Token Audio(MediaOperation operation)
        {
            return new Token(TokenKind.Audio, new object[] { operation });
        }

        public static Token Audio(MediaOperation operation, string path)
        {
            return new Token(TokenKind.Audio, new object[] { operation, path });
        }

        public static Token Image(string path)
        {
            return new Token(TokenKind.Image, new object[] { path });
        }

        public static Token Image(string path, double width, double height)
        {
            return new Token(TokenKind.Image, new object[] { path, width, height });
        }

        public static Token Video(MediaOperation operation)
        {
            return new Token(TokenKind.Video, new object[] { operation });
        }

        public static Token Video(MediaOperation operation, string path)
        {
            return new Token(TokenKind.Video, new object[] { operation, path });
        }

        public static Token Video(MediaOperation operation, string path, double width, double height)
        {
            return new Token(TokenKind.Video, new object[] { operation, path, width, height });
        }

        public static Token BeginQuickSession()
        {
            return new Token(TokenKind.BeginQuickSession);
        }

        public static Token EndQuickSession()
        {
            return new Token(TokenKind.EndQuickSession);
        }

        public static Token DeleteDirectory()
        {
            return new Token(TokenKind.DeleteDirectory);
        }

        public static Token Shutdown()
        {
            return new Token(TokenKind.Shutdown);
        }

        public static Token Disconnection()
        {
            return new Token(TokenKind.Disconnection);
        }

        public static Token Connection()
        {
            return new Token(TokenKind.Connection);
        }

        #endregion
    }
}
