using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

using ImoutoDesktop.Models;
using ImoutoDesktop.Services;

namespace ImoutoDesktop
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App
    {
        private readonly Mutex _mutex = new(false, "ImoutoDesktop");
        private readonly string _default = "sakura";

        public string RootDirectory { get; private set; }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            if (!_mutex.WaitOne(0, false))
            {
                MessageBox.Show("既に起動しています");

                Shutdown();

                return;
            }

#if DEBUG
            RootDirectory = @"C:\Users\shibayan\Documents\GitHub\ImoutoDesktop\resource";
#else
            RootDirectory = System.AppContext.BaseDirectory;
#endif

            // 設定ファイルを読み込む
            Settings.Load(Path.Combine(RootDirectory, "settings.yml"));

            // インストールされているいもうとを読み込む
            CharacterManager.Rebuild(Path.Combine(RootDirectory, "characters"));

            // インストールされている吹き出しを読み込む
            BalloonManager.Rebuild(Path.Combine(RootDirectory, "balloons"));

            // 起動条件を満たしているか確認する
            if (CharacterManager.Characters.Count == 0 || BalloonManager.Balloons.Count == 0)
            {
                // いもうと、吹き出しが存在しない
                MessageBox.Show("いもうとや吹き出しがインストールされていません。");

                // シャットダウン
                Shutdown();

                return;
            }

            // コンテキストを作成して、いもうとを起動
            Context context = null;

            if (Settings.Default.LastCharacter != null)
            {
                context = Context.Create(Settings.Default.LastCharacter);
            }

            context ??= Context.Create(_default) ?? Context.Create(CharacterManager.Characters.First().Key);

            context.Start();
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            if (!string.IsNullOrEmpty(RootDirectory))
            {
                Settings.Save(Path.Combine(RootDirectory, "settings.yml"));
            }

            _mutex.Close();
        }
    }
}
