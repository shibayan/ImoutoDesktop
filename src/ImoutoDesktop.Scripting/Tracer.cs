using System;
using System.IO;
using System.Text;

namespace ImoutoDesktop.Scripting;

internal class Tracer : IDisposable
{
    public Tracer(string path, bool isEnable)
    {
        _isEnable = isEnable;
        if (isEnable)
        {
            _writer = new StreamWriter(path, false, Encoding.UTF8);

            // 時間を記録する
            _writer.WriteLine("{0:yyyy/MM/dd HH:mm:ss}", DateTime.Now);
        }
    }

    private readonly bool _isEnable;
    private readonly StreamWriter _writer;

    public void Dispose()
    {
        if (_isEnable)
        {
            _writer.Dispose();
        }
    }

    public void Close()
    {
        if (_isEnable)
        {
            _writer.Close();
        }
    }

    public void WriteLine(string value)
    {
        _writer.WriteLine(value);
    }

    public void WriteLine(string format, params object[] arg)
    {
        _writer.WriteLine(format, arg);
    }
}
