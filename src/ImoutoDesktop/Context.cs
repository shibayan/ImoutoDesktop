using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ImoutoDesktop.IO;
using ImoutoDesktop.Scripting;
using ImoutoDesktop.Windows;

namespace ImoutoDesktop
{
    public class Context : IEquatable<Context>
    {
        private Context(Character character)
        {
            // いもうとの定義
            Character = character;

            // ルートディレクトリ
            RootDirectory = character.Directory;

            // プロファイルを読み込む
            Profile = Serializer<Profile>.Deserialize(Path.Combine(RootDirectory, "profile.xml"));
            Profile.Age = Profile.Age == 0 ? Character.Age : Profile.Age;
            Profile.TsundereLevel = Profile.TsundereLevel == 0 ? Character.TsundereLevel : Profile.TsundereLevel;

            // バルーン読み込み
            Balloon = BalloonManager.GetBalloon(Profile.LastBalloon);
            Balloon.CanSelect = false;

            // ルートからイメージ用ディレクトリを作る
            SurfaceLoader = new SurfaceLoader(Path.Combine(RootDirectory, "images"));

            // ウィンドウを作成する
            BalloonWindow = new BalloonWindow
            {
                Context = this,
                Balloon = Balloon,
                LocationOffset = Profile.BalloonOffset
            };

            CharacterWindow = new CharacterWindow
            {
                Context = this,
                BalloonWindow = BalloonWindow
            };

            // スクリプトエンジンを作成する
            ScriptEngine = new ScriptEngine(Path.Combine(RootDirectory, "scripts"));
            InitializeScriptEngine();

            // スクリプトプレイヤーを作成
            ScriptPlayer = new ScriptPlayer(this);
        }

        private const string APP_VERSION = "1.03";

        private static readonly object _syncLock = new object();
        private static readonly List<Context> _activeContexts = new List<Context>();

        public static Context Create(Guid id)
        {
            lock (_syncLock)
            {
                Character character;
                if (CharacterManager.TryGetCharacter(id, out character))
                {
                    character.CanSelect = false;
                    var context = new Context(character);
                    _activeContexts.Add(context);
                    return context;
                }
                return null;
            }
        }

        public static void Delete(Context context)
        {
            lock (_syncLock)
            {
                context.Character.CanSelect = true;
                _activeContexts.Remove(context);
                if (_activeContexts.Count != 0)
                {
                    return;
                }
                // 設定を保存
                Settings.Default.LastCharacter = context.Character.ID;
                // 起動中のコンテキストが 1 つも無くなったら終了
                Application.Current.Shutdown();
            }
        }

        public string RootDirectory { get; }

        public Balloon Balloon { get; private set; }

        public Character Character { get; }

        public Profile Profile { get; }

        public SurfaceLoader SurfaceLoader { get; }

        public CharacterWindow CharacterWindow { get; }

        public BalloonWindow BalloonWindow { get; }

        public ScriptEngine ScriptEngine { get; }

        public ScriptPlayer ScriptPlayer { get; }

        public int HistoryIndex { get; set; } = -1;

        public List<string> CommandHistory { get; } = new();

        private Commands.ICommand _callCommand;

        public void Run()
        {
            // メニューのコマンドを定義する
            var contextMenu = (ContextMenu)Application.Current.Resources["ContextMenuKey"];
            contextMenu.CommandBindings.Clear();
            contextMenu.CommandBindings.Add(new CommandBinding(Input.Commands.Character, CharacterCommand_Executed));
            contextMenu.CommandBindings.Add(new CommandBinding(Input.Commands.Balloon, BalloonCommand_Executed));
            contextMenu.CommandBindings.Add(new CommandBinding(Input.Commands.Option, OptionCommand_Executed));
            contextMenu.CommandBindings.Add(new CommandBinding(Input.Commands.Version, VersionCommand_Executed));
            contextMenu.CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, CloseCommand_Executed));
            // コマンド追加
            _callCommand = new Remoting.CallName(Character.Name);
            Commands.CommandManager.Add(_callCommand);
            // 初期サーフェスを表示して起動
            CharacterWindow.ChangeSurface(0);
            BalloonWindow.Show();
            PlayInvoke("OnBoot");
        }

        public void Close()
        {
            PlayInvoke("OnClose");
        }

        public void Shutdown()
        {
            // メニュー周りクリーンアップ
            var contextMenu = (ContextMenu)Application.Current.Resources["ContextMenuKey"];
            contextMenu.CommandBindings.Clear();
            // コマンド削除
            Commands.CommandManager.Remove(_callCommand);
            // リソースを破棄する
            CharacterWindow.Close();
            BalloonWindow.Close();
            ScriptPlayer.Stop();
            ScriptEngine.Dispose();
            // プロファイルを保存
            Profile.LastBalloon = Balloon.ID;
            Profile.BalloonOffset = BalloonWindow.LocationOffset;
            Serializer<Profile>.Serialize(Path.Combine(RootDirectory, "profile.xml"), Profile);
            // コンテキストを削除
            Delete(this);
        }

        public void PlayInvoke(string id)
        {
            var result = ScriptEngine.Invoke(id);
            var script = CreateScript();
            Commands.CommandManager.ExecuteTranslate(ref result);
            script.AppendLine(Script.Scope.Imouto, result);
            if (id == "OnClose")
            {
                script.AppendLine(Script.Scope.System, @"\-");
            }
            ScriptPlayer.Play(script);
        }

        public void ExecCommand(string input)
        {
            var script = CreateScript();
            script.AppendLine(Script.Scope.User, input.Replace(@"\", @"\\"));
            ScriptEngine.Connecting = Remoting.ConnectionPool.IsConnected;
            var command = Commands.CommandManager.Get(input);
            // コマンドが見つかったか調べる
            if (command != null)
            {
                // コマンド実行前準備
                var canExecute = command.PreExecute(input);
                // スクリプトを実行
                var id = "On" + (command.EventID ?? command.GetType().Name) + (!canExecute ? "Failure" : "");
                string message = null;
                var result = ScriptEngine.Invoke(id, command.Parameters);
                // 実際にコマンドを実行するか判別
                if (!ScriptEngine.Reject && canExecute)
                {
                    // コマンドを実行する
                    if (!command.Execute(input, out message))
                    {
                        id = "On" + ((command.EventID ?? command.GetType().Name) + "Failure");
                        result = ScriptEngine.Invoke(id, command.Parameters);
                    }
                }
                // いもうとの反応を追加する
                if (!string.IsNullOrEmpty(result))
                {
                    Commands.CommandManager.ExecuteTranslate(ref result);
                    script.AppendLine(Script.Scope.Imouto, result);
                }
                // 実行結果が存在すれば追加する
                if (!string.IsNullOrEmpty(message))
                {
                    script.AppendLine(Script.Scope.System, message);
                }
                // 終了コマンドなら終了する
                if (id == "OnClose")
                {
                    script.AppendLine(Script.Scope.System, @"\-");
                }
            }
            else
            {
                // コマンドが存在しない
                var result = ScriptEngine.Invoke("OnUnknownCommand", input);
                // いもうとの反応を追加する
                if (!string.IsNullOrEmpty(result))
                {
                    Commands.CommandManager.ExecuteTranslate(ref result);
                    script.AppendLine(Script.Scope.Imouto, result);
                }
            }
            // スクリプトを再生
            ScriptPlayer.Play(script);
        }

        public Script CreateScript()
        {
            var script = new Script
            {
                UserName = Settings.Default.UserName,
                ImoutoName = Character.Name,
                Honorific = Settings.Default.Honorific,
                UserColor = Balloon.UserColor,
                ImoutoColor = Balloon.ImoutoColor
            };

            return script;
        }

        private void InitializeScriptEngine()
        {
            ScriptEngine.Age = Profile.Age;
            ScriptEngine.TsundereLevel = Profile.TsundereLevel;
            ScriptEngine.Connecting = false;
            ScriptEngine.AllowOperate = Settings.Default.AllowImoutoAllOperation;
        }

        private void CharacterCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var id = (Guid)e.Parameter;
            var context = Create(id);
            if (context == null)
            {
                return;
            }
            Shutdown();
            context.Run();
        }

        private void BalloonCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var id = (Guid)e.Parameter;

            if (!BalloonManager.TryGetBalloon(id, out var balloon))
            {
                return;
            }

            Balloon.CanSelect = true;
            balloon.CanSelect = false;

            Balloon = balloon;
            BalloonWindow.Balloon = balloon;
        }

        private void OptionCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new SettingDialog
            {
                Age = Profile.Age,
                TsundereLevel = Profile.TsundereLevel
            };

            if (dialog.ShowDialog() ?? false)
            {
                Profile.Age = dialog.Age;
                Profile.TsundereLevel = dialog.TsundereLevel;
                InitializeScriptEngine();
            }
        }

        private void VersionCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var assembly = Assembly.GetEntryAssembly();
            var attributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);

            if (attributes.Length > 0)
            {
                var title = ((AssemblyTitleAttribute)attributes[0]).Title;
                MessageBox.Show(
                    $"{title} ver.{APP_VERSION}",
                    "バージョン情報",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        #region IEquatable<Context> メンバ

        public bool Equals(Context other)
        {
            return Character.ID == other.Character.ID;
        }

        #endregion
    }
}
