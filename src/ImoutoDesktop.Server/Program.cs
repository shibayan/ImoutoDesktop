using System;
using System.Windows.Forms;

namespace ImoutoDesktop.Server
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            var mutex = new System.Threading.Mutex(false, "ImoutoDesktop.Server");
            if (!mutex.WaitOne(0, false))
            {
                MessageBox.Show("既に起動しています");
                return;
            }
            GC.KeepAlive(mutex);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            mutex.Close();
        }
    }
}
