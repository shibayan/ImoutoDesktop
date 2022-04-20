namespace ImoutoDesktop.Scripting;

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
        return new Token(TokenKind.Text, text);
    }

    public static Token LineBreak()
    {
        return new Token(TokenKind.LineBreak);
    }

    public static Token Font(FontOperation operation, object value)
    {
        return new Token(TokenKind.Font, new[] { operation, value });
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

    public static Token Connect()
    {
        return new Token(TokenKind.Connect);
    }

    public static Token Disconnect()
    {
        return new Token(TokenKind.Disconnect);
    }

    #endregion
}
