namespace ImoutoDesktop.Scripting
{
    public class Token
    {
        public Token(TokenKind kind)
        {
            Kind = kind;
        }

        private Token(TokenKind kind, object value)
        {
            Kind = kind;
            Value = value;
        }

        private Token(TokenKind kind, object[] parameters)
        {
            Kind = kind;
            Parameters = parameters;
        }

        public TokenKind Kind { get; }

        /// <summary>
        /// タグが保持する値。
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// タグの引数。
        /// </summary>
        public object[] Parameters { get; }

        #region Token のファクトリ

        public static Token Text(string text)
        {
            return new(TokenKind.Text, text);
        }

        public static Token LineBreak()
        {
            return new(TokenKind.LineBreak);
        }

        public static Token Font(FontOperation operation, object value)
        {
            return new(TokenKind.Font, new object[] { operation, value });
        }

        public static Token Clear()
        {
            return new(TokenKind.Clear);
        }

        public static Token Surface(int id)
        {
            return new(TokenKind.Surface, id);
        }

        public static Token Sleep(int millisecond)
        {
            return new(TokenKind.Sleep, millisecond);
        }

        public static Token BringToFront()
        {
            return new(TokenKind.BringToFront);
        }

        public static Token Exit()
        {
            return new(TokenKind.Exit);
        }

        public static Token Audio(MediaOperation operation)
        {
            return new(TokenKind.Audio, new object[] { operation });
        }

        public static Token Audio(MediaOperation operation, string path)
        {
            return new(TokenKind.Audio, new object[] { operation, path });
        }

        public static Token Image(string path)
        {
            return new(TokenKind.Image, new object[] { path });
        }

        public static Token Image(string path, double width, double height)
        {
            return new(TokenKind.Image, new object[] { path, width, height });
        }

        public static Token Video(MediaOperation operation)
        {
            return new(TokenKind.Video, new object[] { operation });
        }

        public static Token Video(MediaOperation operation, string path)
        {
            return new(TokenKind.Video, new object[] { operation, path });
        }

        public static Token Video(MediaOperation operation, string path, double width, double height)
        {
            return new(TokenKind.Video, new object[] { operation, path, width, height });
        }

        public static Token BeginQuickSession()
        {
            return new(TokenKind.BeginQuickSession);
        }

        public static Token EndQuickSession()
        {
            return new(TokenKind.EndQuickSession);
        }

        public static Token DeleteDirectory()
        {
            return new(TokenKind.DeleteDirectory);
        }

        public static Token Shutdown()
        {
            return new(TokenKind.Shutdown);
        }

        public static Token Disconnection()
        {
            return new(TokenKind.Disconnection);
        }

        public static Token Connection()
        {
            return new(TokenKind.Connection);
        }

        #endregion
    }
}
