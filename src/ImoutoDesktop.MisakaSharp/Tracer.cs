using System;
using System.IO;
using System.Text;

namespace ImoutoDesktop.MisakaSharp
{
    class Tracer : IDisposable
    {
        public Tracer(string path, bool isEnable)
        {
            this.isEnable = isEnable;
            if (isEnable)
            {
                writer = new StreamWriter(path, false, Encoding.UTF8);
                // 時間を記録する
                writer.WriteLine("{0}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            }
        }

        private bool isEnable;
        private StreamWriter writer;

        public void Dispose()
        {
            if (isEnable)
            {
                writer.Dispose();
            }
        }

        public void Close()
        {
            if (isEnable)
            {
                writer.Close();
            }
        }

        public void WriteLine(string value)
        {
            writer.WriteLine(value);
        }

        public void WriteLine(string format, params object[] arg)
        {
            writer.WriteLine(format, arg);
        }
    }
}
