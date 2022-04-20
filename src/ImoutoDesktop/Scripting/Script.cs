using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace ImoutoDesktop.Scripting;

public class Script : IEnumerable<Token>
{
    private string _script;

    private readonly StringBuilder _buffer = new();

    public string UserName { get; set; }

    public string ImoutoName { get; set; }

    public string Honorific { get; set; }

    public string UserColor { get; set; }

    public string ImoutoColor { get; set; }

    public void AppendLine() => _buffer.Append(@"\n");

    public void AppendLine(Scope scope, string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        if (scope == Scope.Character)
        {
            foreach (var item in Split(text))
            {
                _buffer.Append($@"\f[color,{ImoutoColor}]\q\f[bold,true]{ImoutoName}\f[bold,false] : \q{item}\f[color,reset]\w9\n");
            }
        }
        else if (scope == Scope.User)
        {
            _buffer.Append($@"\f[color,{UserColor}]\q\f[bold,true]{UserName}\f[bold,false] : \q{text}\f[color,reset]\w9\n");
        }
        else
        {
            _buffer.Append($@"\q{text}\n\q");
        }
    }

    public void AppendLine(Scope scope, string text, params object[] args)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        if (scope == Scope.Character)
        {
            foreach (var item in Split(string.Format(text, args)))
            {
                _buffer.Append($@"\f[color,{ImoutoColor}]\q\f[bold,true]{ImoutoName}\f[bold,false] : \q{item}\f[color,reset]\w9\n");
            }
        }
        else if (scope == Scope.User)
        {
            _buffer.Append($@"\f[color,{UserColor}]\q\f[bold,true]{UserName}\f[bold,false] : \q{string.Format(text, args)}\f[color,reset]\w9\n");
        }
        else
        {
            _buffer.Append($@"\q{string.Format(text, args)}\n\q");
        }
    }

    public char this[int index]
    {
        get => _buffer[index];
    }

    public int Length => _buffer.Length;

    public override string ToString() => _buffer.ToString();

    private string[] Split(string text)
    {
        int i;
        var previndex = 0;
        var token = new List<string>();
        for (i = 0; i < text.Length; i++)
        {
            if (text[i] == '\\')
            {
                i += 1;
                if (text[i] == 'n')
                {
                    token.Add(text.Substring(previndex, i - previndex - 1));
                    previndex = i + 1;
                }
            }
        }
        if (i != previndex)
        {
            token.Add(text.Substring(previndex, i - previndex));
        }
        return token.ToArray();
    }

    private bool GetParameter(ref int index, out string[] param)
    {
        if (_script.Length <= index + 1 || _script[index + 1] != '[')
        {
            // スクウェアブラケットじゃなかった場合は false で終わる
            param = null;
            return false;
        }
        var previndex = index + 2;
        var inDoubleQuote = false;
        var parameter = new List<string>();
        for (index = previndex; index < _script.Length; index++)
        {
            if (_script[index] == '\"')
            {
                if (index == 0 || _script[index - 1] != '\\')
                {
                    inDoubleQuote = !inDoubleQuote;
                }
            }
            else if (!inDoubleQuote)
            {
                if (_script[index] == ']')
                {
                    if (previndex != index)
                    {
                        parameter.Add(_script.Substring(previndex, index - previndex));
                    }
                    break;
                }
                if (_script[index] == ',')
                {
                    if (previndex != index)
                    {
                        parameter.Add(_script.Substring(previndex, index - previndex));
                    }
                    previndex = index + 1;
                }
            }
        }
        param = parameter.ToArray();
        return true;
    }

    private FontOperation ParseFontOperation(string str)
    {
        return str switch
        {
            "color" => FontOperation.Color,
            "family" => FontOperation.FontFamily,
            "size" => FontOperation.Size,
            "bold" => FontOperation.Weight,
            _ => FontOperation.Color
        };
    }

    private static MediaOperation ParseMediaOperation(string str)
    {
        return str switch
        {
            "play" => MediaOperation.Play,
            "stop" => MediaOperation.Stop,
            "pause" => MediaOperation.Pause,
            "wait" => MediaOperation.Wait,
            _ => MediaOperation.Play
        };
    }

    public enum Scope
    {
        User,
        Character,
        System
    }

    #region IEnumerable<Token> メンバ

    public IEnumerator<Token> GetEnumerator()
    {
        var isQuickSession = false;

        _script = _buffer.ToString();

        if (_script.EndsWith(@"\n"))
        {
            _script = _script.Substring(0, _script.Length - 2);
        }

        for (var i = 0; i < _script.Length; ++i)
        {
            if (_script[i] == '\\')
            {
                // タグ
                i += 1;
                string[] param;
                switch (_script[i])
                {
                    case 'c':
                        // クリア
                        yield return Token.Clear();
                        break;
                    case 'f':
                        // フォント変更
                        if (GetParameter(ref i, out param))
                        {
                            if (param.Length < 1)
                            {
                                break;
                            }
                            var operation = ParseFontOperation(param[0]);
                            switch (operation)
                            {
                                case FontOperation.Color:
                                    if (param[1] != "reset")
                                    {
                                        yield return Token.Font(operation, ColorConverter.ConvertFromString(param[1]));
                                    }
                                    else
                                    {
                                        yield return Token.Font(operation, null);
                                    }
                                    break;
                                case FontOperation.FontFamily:
                                    if (param[1] != "reset")
                                    {
                                        yield return Token.Font(operation, param[1]);
                                    }
                                    else
                                    {
                                        yield return Token.Font(operation, null);
                                    }
                                    break;
                                case FontOperation.Size:
                                    if (param[1] != "reset")
                                    {
                                        yield return Token.Font(operation, double.Parse(param[1]));
                                    }
                                    else
                                    {
                                        yield return Token.Font(operation, null);
                                    }
                                    break;
                                case FontOperation.Weight:
                                    yield return Token.Font(operation, bool.Parse(param[1]));
                                    break;
                            }
                        }
                        break;
                    case 'n':
                        // テキスト改行
                        yield return Token.LineBreak();
                        break;
                    case 'q':
                        // クイックセッション
                        if (isQuickSession)
                        {
                            // クイックセッション終了
                            yield return Token.EndQuickSession();
                        }
                        else
                        {
                            // クイックセッション開始
                            yield return Token.BeginQuickSession();
                        }
                        isQuickSession = !isQuickSession;
                        break;
                    case 's':
                        // サーフェス切り替え
                        if (GetParameter(ref i, out param))
                        {
                            yield return Token.Surface(int.Parse(param[0]));
                        }
                        break;
                    case 'v':
                        // 最前面に移動
                        yield return Token.BringToFront();
                        break;
                    case 'w':
                        // ウェイト
                        i += 1;
                        if (char.IsDigit(_script[i]))
                        {
                            yield return Token.Sleep(50 * int.Parse(_script.Substring(i, 1)));
                        }
                        break;
                    case '-':
                        yield return Token.Exit();
                        break;
                    case '\\':
                        // '\'を表示
                        yield return Token.Text(@"\");
                        break;
                    case '%':
                        // '%'を表示
                        yield return Token.Text("%");
                        break;
                    case 'r':
                        if (GetParameter(ref i, out param))
                        {
                            if (param.Length < 1)
                            {
                                break;
                            }
                            switch (param[0])
                            {
                                case "delete":
                                    switch (param[1])
                                    {
                                        case "directory":
                                            yield return Token.DeleteDirectory();
                                            break;
                                    }
                                    break;
                                case "shutdown":
                                    yield return Token.Shutdown();
                                    break;
                                case "connection":
                                    yield return Token.Connect();
                                    break;
                                case "disconnection":
                                    yield return Token.Disconnect();
                                    break;
                            }
                        }
                        break;
                    case '_':
                        i += 1;
                        switch (_script[i])
                        {
                            case 'a':
                                // 音楽再生
                                // \_a[play/stop/pause/wait,path]
                                if (GetParameter(ref i, out param))
                                {
                                    if (param.Length < 1)
                                    {
                                        break;
                                    }
                                    var operation = ParseMediaOperation(param[0]);
                                    switch (param.Length)
                                    {
                                        case 1:
                                            yield return Token.Audio(operation);
                                            break;
                                        case 2:
                                            yield return Token.Audio(operation, param[1]);
                                            break;
                                    }
                                }
                                break;
                            case 'i':
                                // バルーンに画像貼り付け
                                if (GetParameter(ref i, out param))
                                {
                                    switch (param.Length)
                                    {
                                        case 1:
                                            yield return Token.Image(param[0]);
                                            break;
                                        case 3:
                                            yield return Token.Image(param[0], double.Parse(param[1]), double.Parse(param[2]));
                                            break;
                                    }
                                }
                                break;
                            case 'v':
                                // バルーンに動画貼り付け
                                if (GetParameter(ref i, out param))
                                {
                                    if (param.Length < 1)
                                    {
                                        break;
                                    }
                                    var operation = ParseMediaOperation(param[0]);
                                    switch (param.Length)
                                    {
                                        case 1:
                                            yield return Token.Video(operation);
                                            break;
                                        case 2:
                                            yield return Token.Video(operation, param[1]);
                                            break;
                                        case 4:
                                            yield return Token.Video(operation, param[1], double.Parse(param[2]), double.Parse(param[3]));
                                            break;
                                    }
                                }
                                break;
                            case 'w':
                                // 高精度ウェイト
                                if (GetParameter(ref i, out param))
                                {
                                    yield return Token.Sleep(int.Parse(param[0]));
                                }
                                break;
                        }
                        break;
                }
            }
            else if (_script[i] == '%')
            {
                if (string.Compare(_script, i + 1, "username", 0, 8) == 0)
                {
                    yield return Token.Text(UserName + Honorific);
                    i += 8;
                }
                else
                {
                    yield return Token.Text("%");
                }
            }
            else
            {
                // テキスト
                var end = _script.IndexOfAny(new[] { '\\', '%' }, i);
                if (end == -1)
                {
                    end = _script.Length;
                }
                yield return Token.Text(_script.Substring(i, end - i));
                i = end - 1;
            }
        }
    }

    #endregion

    #region IEnumerable メンバ

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion
}
